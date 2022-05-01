//#define DEBUG_PACKET_LOSS

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace WinTest
{
    public class WtLogSync : WinTestLogBase
    {
        private IPAddress localbroadcastIP;

        private wtListener wtl;

        private System.Timers.Timer ti_get_log;

        public WtLogSync(LogWriteMessageDelegate mylog) : base(mylog)
        {
            localbroadcastIP = WinTest.GetIpIFBroadcastAddress();
            wtl = new wtListener(WinTest.WinTestDefaultPort);
            wtl.wtMessageReceived += wtMessageReceivedHandler;

            QSO.Columns.Add("RUNSTN");
            QSO.Columns.Add("LOGID");
            QSO.Columns.Add("LOGNR", typeof(uint));
            DataColumn[] QSOkeys = new DataColumn[]
            {
                QSO.Columns["RUNSTN"],
                QSO.Columns["LOGID"],
                QSO.Columns["LOGNR"]
            };
            QSO.PrimaryKey = QSOkeys;

            wtlogsyncState = WTLOGSYNCSTATE.WAIT_HELLO;

            ti_get_log = new System.Timers.Timer();
            ti_get_log.Enabled = true;
            ti_get_log.Interval = 10000; // check every 10s
            ti_get_log.Elapsed += new System.Timers.ElapsedEventHandler(this.ti_ti_get_log_Tick);
        }

        public override void Dispose()
        {
            ti_get_log.Stop();
            wtl.close();
        }
        private string my_wtname;

        private void send(wtMessage Msg)
        {
            try
            {
                UdpClient client = new UdpClient();
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
                client.Client.ReceiveTimeout = 10000;
                IPEndPoint groupEp = new IPEndPoint(localbroadcastIP, WinTest.WinTestDefaultPort);
                client.Connect(groupEp);
                //Console.WriteLine("send: " + Msg.Data);
                byte[] b = Msg.ToBytes();
                client.Send(b, b.Length);
                client.Close();
            }
            catch
            {
            }
        }

        private enum WTLOGSYNCSTATE { WAIT_HELLO, HELLO_RECEIVED, GET_QSO, QSO_IN_SYNC };

        private WTLOGSYNCSTATE _wtlogsyncState = WTLOGSYNCSTATE.WAIT_HELLO;
        private WTLOGSYNCSTATE wtlogsyncState
        {
            get { return _wtlogsyncState; }
            set
            {
                if (_wtlogsyncState != value)
                {
                    _wtlogsyncState = value;
                    // TODO emit event...
                }
            }
        }

        private class logSegment
        {
            public uint count_from;  /// <value>first ID</value>   
            public uint count_to;   /// <value>last ID in consecutive list</value>
        }

        private class logState
        {
            public string StationUniqueID;
            public List<logSegment> ls;

            public logState(string StationUniqueID, List<logSegment> ls)
            {
                this.StationUniqueID = StationUniqueID;
                this.ls = ls;
            }
        }

        private class StationLogStat : logState
        {
            public enum wtAvailableFrom { Owner, LoggedElse };
            public wtAvailableFrom AvailableFrom;

            public StationLogStat( string StationUniqueID, wtAvailableFrom AvailableFrom, List<logSegment> ls)
                :base(StationUniqueID, ls)
            {

                this.AvailableFrom = AvailableFrom;
            }
        }

        private ulong StationProtocolVersion = 0;
        private uint stationContestID = 0;

        private class StationSyncStatus : wtStatus.wtStat
        {
            public DateTime first_QSO_ts;

            public StationSyncStatus(string from, DateTime first_QSO_ts)
                :base(from, "", "", 0, 0)
            {
                this.first_QSO_ts = first_QSO_ts;
                this.logstat = new List<StationLogStat>();
            }
            public List<StationLogStat> logstat;
        }

        private List<StationSyncStatus> wtStationSyncList = new List<StationSyncStatus>();

        private List<logState> myLogState = new List<logState>();

        private void send_status()
        {
            wtMessage Msg = new wtMessage(WTMESSAGES.STATUS, my_wtname, "", "0 12 0 0 0 \"0\" 0 \"1\" 0 \"\""); //FIXME really const or need to adapt
            send(Msg);
        }

        private void send_needqso(string target_wt, string StationUniqueID, uint count_from, uint count_to)
        {
            wtMessage Msg = new wtMessage(WTMESSAGES.NEEDQSO, my_wtname, target_wt, string.Concat(new string[]
            {
                " \"", StationUniqueID, "\" ", count_from.ToString(), " ", count_to.ToString()
            }));
            send(Msg);
        }

        private bool find_qsos_count_to(ref logState lst)
        {
            lst.ls = new List<logSegment>();
            lst.ls.Add(new logSegment());
            lst.ls[0].count_from = 0;
            lst.ls[0].count_to = 0;

            string StationName = lst.StationUniqueID.Split('@')[0];
            string LogId = lst.StationUniqueID.Split('@')[1];
            var rows = QSO.Select(string.Format("([RUNSTN] = '{0}') AND ([LOGID] = '{1}')", StationName, LogId));
            if (rows!= null && rows.Count() > 0)
            {
                try
                {
                    List<uint> lognrs = new List<uint>();

                    for (int i = 0; i < rows.Count(); i++)
                    {
                         lognrs.Add((uint)rows[i]["LOGNR"]);
                    }
                    lognrs.Sort();
                    lst.ls[0].count_from = lognrs[0];

                    int segment = 0;
                    int count_from = (int)lognrs[0];

                    for (int i = 1; i < lognrs.Count(); i++)
                        if (lognrs[i] != i + count_from)
                        {
                            lst.ls[segment].count_to = lognrs[i - 1];
                            segment++;
                            lst.ls.Add(new logSegment());
                            lst.ls[segment].count_from = lognrs[i];
                            count_from = (int)lognrs[i] - i;
                        }
                    lst.ls[segment].count_to = lognrs.Last();

                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message );
                }
            }
            return false;
        }

        private string needQSOsentForStationID;
        private uint needQSOsentForCountTo = 0;
        private DateTime needQSOSent_TimeStamp;

        private enum SECTION_STATE { SECTION_DONE, NEED_QSO_SENT, SECTION_TIMEOUT };
        private SECTION_STATE check_section(StationLogStat lst, string stationname)
        {
            var myls = myLogState.Find(x => x.StationUniqueID.Equals(lst.StationUniqueID));

            uint count_to, count_from;

            // first time, request first part?
            count_from = lst.ls[0].count_from;
            count_to = lst.ls[0].count_to;

            if (myls != null)
            {
                // check if we are done
                if (lst.ls.Count ==  myls.ls.Count)
                {
                    bool done = true;

                    for(int i = 0; i<lst.ls.Count; i++ )
                    {
                        if (lst.ls[i].count_from != myls.ls[i].count_from
                            || lst.ls[i].count_to != myls.ls[i].count_to )
                        {
                            done = false;
                            break;
                        }
                    }
                    if (done) 
                        return SECTION_STATE.SECTION_DONE;
                }
                // send NEEDQSO, we are missing parts

                int segment = 0;
                while (segment < lst.ls.Count && segment < myls.ls.Count &&
                    lst.ls[segment].count_from == myls.ls[segment].count_from &&
                    lst.ls[segment].count_to == myls.ls[segment].count_to)
                    segment++;

                if (segment < myls.ls.Count)
                {
                    // we miss parts of the current segment
                    if (myls.ls[segment].count_to < lst.ls[segment].count_to)
                    {
                        // we are missing parts at the end of the curent segment
                        count_from = myls.ls[segment].count_to + 1;
                        count_to = lst.ls[segment].count_to;
                        // maybe this is in the next segment?
                        if (segment + 1 < myls.ls.Count && count_to > myls.ls[segment + 1].count_from)
                        {
                            count_to = myls.ls[segment + 1].count_from - 1;
                            if (count_to < count_from)
                                count_from = count_to; // just one
                        }
                    }
                    else if (myls.ls[segment].count_from > lst.ls[segment].count_from)
                    {
                        // we miss parts at the beginning
                        count_from = lst.ls[segment].count_from;
                        count_to = myls.ls[segment].count_from - 1;
                    }
                }
                else
                {
                    // we take the data from the next segment
                    count_from = lst.ls[segment].count_from;
                    count_to = lst.ls[segment].count_to;
                }
            }

            if (count_from > count_to)
                count_from = count_to;
            if (count_to > count_from + 49)
                count_to = count_from + 49; // max 50
            // check if these haven't been requested yet
            if ((lst.StationUniqueID == needQSOsentForStationID &&
                DateTime.UtcNow.Subtract(needQSOSent_TimeStamp).TotalMilliseconds > 5000))
            {
                needQSOsentForStationID = "";
                // station did not answer in time
                return SECTION_STATE.SECTION_TIMEOUT;
            }
#if DEBUG_PACKET_LOSS
            if (skip_paket())
                Console.WriteLine("skip send_needqso " + stationname + " " + lst.StationUniqueID + " " + count_from + "-" + count_to);
            else
#endif
            send_needqso(stationname, lst.StationUniqueID, count_from, count_to);

            needQSOSent_TimeStamp = DateTime.UtcNow;

            needQSOsentForStationID = lst.StationUniqueID;
            needQSOsentForCountTo = count_to;

            return SECTION_STATE.NEED_QSO_SENT;
        }

        private bool find_alternative_source(string original_from, string StationUniqueID)
        {
            // we need to find another source:
            foreach (var sl2 in wtStationSyncList)
            {
                if (sl2.from == original_from)
                    continue; // skip the failing station

                var ls2 = sl2.logstat.Find(x => x.StationUniqueID == StationUniqueID);
                if (ls2 != null)
                {
                    Console.WriteLine(StationUniqueID + " try " + sl2.from + " " + DateTime.Now.ToString("h:mm:ss"));
                    var cs2 = check_section(ls2, sl2.from);
                    if (cs2 == SECTION_STATE.NEED_QSO_SENT)
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine(StationUniqueID + " alternative after timeout " + cs2.ToString() + " " + DateTime.Now.ToString("h:mm:ss"));
                        original_from = sl2.from; // avoid this one now
                    }
                }
                else
                    Console.WriteLine(StationUniqueID + " no alternative in " + sl2.from);
            }
            return false;
        }

        private bool intimer = false;
        private void ti_ti_get_log_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (intimer)
                    return;
                if (wtlogsyncState != WTLOGSYNCSTATE.GET_QSO && wtlogsyncState != WTLOGSYNCSTATE.QSO_IN_SYNC)
                    return;

                intimer = true;

                ti_get_log.Interval = 10000; // check every 10s


                // iterate over all logstat lists
                bool needQSOs_sent = false;

                foreach (var sl in wtStationSyncList)
                {
                    foreach (var ls in sl.logstat)
                    {
                        // prefer owner on first iteration
                        if (ls.AvailableFrom == StationLogStat.wtAvailableFrom.Owner)
                        {
                            var cs = check_section(ls, sl.from);
                            if (cs == SECTION_STATE.SECTION_DONE)
                            {
                                Console.WriteLine(ls.StationUniqueID + " done (owner) " + DateTime.Now.ToString("h:mm:ss"));
                            }
                            else if (cs == SECTION_STATE.SECTION_TIMEOUT)
                            {
                                Console.WriteLine(ls.StationUniqueID + " timeout " + DateTime.Now.ToString("h:mm:ss"));
                                // we need to find another source:
                                if (find_alternative_source(sl.from, ls.StationUniqueID))
                                {
                                    needQSOs_sent = true;
                                    goto exitLoop;
                                }
                            }
                            else if (cs == SECTION_STATE.NEED_QSO_SENT)
                            {
                                needQSOs_sent = true;
                                goto exitLoop;
                            }
                        }
                    }
                }
                foreach (var sl in wtStationSyncList)
                {
                    foreach (var ls in sl.logstat)
                    {
                        // done owner above, so only use what is left
                        if (ls.AvailableFrom != StationLogStat.wtAvailableFrom.Owner)
                        {
                            var cs = check_section(ls, sl.from);
                            if (cs == SECTION_STATE.SECTION_DONE)
                            {
                                Console.WriteLine(ls.StationUniqueID + " done " + DateTime.Now.ToString("h:mm:ss"));
                            }
                            else if (cs == SECTION_STATE.SECTION_TIMEOUT)
                            {
                                Console.WriteLine(ls.StationUniqueID + " timeout " + DateTime.Now.ToString("h:mm:ss"));
                                // we need to find another source:
                                if (find_alternative_source(sl.from, ls.StationUniqueID))
                                {
                                    needQSOs_sent = true;
                                    goto exitLoop;
                                }
                            }
                            else if (cs == SECTION_STATE.NEED_QSO_SENT)
                            {
                                needQSOs_sent = true;
                                goto exitLoop;
                            }
                        }
                    }
                }
                exitLoop:
                if (!needQSOs_sent)
                {
                    if (wtlogsyncState == WTLOGSYNCSTATE.GET_QSO)
                        wtlogsyncState = WTLOGSYNCSTATE.QSO_IN_SYNC;
                    Console.WriteLine("all done " + QSO.Rows.Count
                                      + " 432: "
                                      + QSO.Select("[BAND]='432M'").Length
                                      + " 1296: "
                                      + QSO.Select("[BAND]='1_2G'").Length
                                      + " 2.3: "
                                      + QSO.Select("[BAND]='2_3G'").Length
                                      + " 5.7: "
                                      + QSO.Select("[BAND]='5_7G'").Length
                                      + " 10: "
                                      + QSO.Select("[BAND]='10G'").Length
                                      + " 24: "
                                      + QSO.Select("[BAND]='24G'").Length
                    );
                    var log13cm = QSO.Select("[BAND]='2_3G'");
                    //                foreach (var entry in log13cm)
                    //                    Console.WriteLine(entry.ItemArray[0] + " " + entry.ItemArray[2] + " " + entry.ItemArray[3]);
                }
                else
                {
                    if (wtlogsyncState == WTLOGSYNCSTATE.QSO_IN_SYNC)
                        wtlogsyncState = WTLOGSYNCSTATE.GET_QSO;
                }
            }
            catch (Exception ex)
            {
                // FIXME: the foreach above sometimes throw an exception as the underlying data changed... Locking? Local copy?
                Error(System.Reflection.MethodBase.GetCurrentMethod().Name, " " + ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                intimer = false;
            }
        }


        public override string getStatus()
        {
            return QSO.Rows.Count.ToString();
        }

        public override void Get_QSOs(string wtname)
        {
            // TODO: start log sync
            my_wtname = wtname;
            if (wtlogsyncState == WTLOGSYNCSTATE.HELLO_RECEIVED)
                wtlogsyncState = WTLOGSYNCSTATE.GET_QSO;
        }

#if DEBUG_PACKET_LOSS
        private Random rnd = new Random();
        private bool skip_paket()
        {
            return rnd.Next() < (int.MaxValue / 16);
        }
#endif
        private void wtMessageReceivedHandler(object sender, wtListener.wtMessageEventArgs e)
        {
            Console.WriteLine("WT Msg " + e.Msg.Msg + " src " +
            e.Msg.Src + " dest " + e.Msg.Dst + " data " + e.Msg.Data + " checksum "
            + e.Msg.HasChecksum);
            if (e.Msg.Msg == WTMESSAGES.HELLO && e.Msg.HasChecksum)
            {
                /*
                        "StationName" "ToStation" <from:initClock> <ProtocolVersion> "MASTER|SLAVE" \
                              <ContestID> <ModeCategoryID> <FirstQsoTimeStamp>
                 */
                string[] data = e.Msg.Data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                DateTime ts = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                ts = ts.AddSeconds((double)Int32.Parse(data[0]));

                ulong protocol_version;
                if (!UInt64.TryParse(data[1], out protocol_version))
                    protocol_version = 0;
                string master = data[2];
                uint contest_ID;
                if (!UInt32.TryParse(data[3], out contest_ID))
                    contest_ID = 0;
                uint mode_category;
                if (!UInt32.TryParse(data[4], out mode_category))
                    mode_category = 0;
                DateTime first_QSO_ts = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                first_QSO_ts = first_QSO_ts.AddSeconds((double)Int32.Parse(data[5]));
                /*
                 * WT Msg HELLO src mm dest  data 11290 130 SLAVE 20 2 1601733638 checksum True
                 * WT Msg HELLO src mm2 dest  data 2634 130 SLAVE 20 2 1601733638 checksum True
                 */
                Console.WriteLine("HELLO: from " + e.Msg.Src + " protocol_version " + protocol_version + " master " + master +
                   " contest_ID " + contest_ID + " mode_category " + mode_category +
                   " first_QSO_ts " + first_QSO_ts.ToShortDateString() + " " + first_QSO_ts.ToShortTimeString());
 
                // we monitor the protocol version and contest ID - if it changes, we better restart to load the log
                // TODO: not enough... be more clever, maybe use first_QSO_ts?
                if ((StationProtocolVersion != protocol_version) || (stationContestID != contest_ID))
                {
                    // if it was 0, we can set it now, if not, restart
                    if (StationProtocolVersion != 0 && stationContestID != 0)
                    {
                        // clear entries
                        wtStationSyncList.Clear();
                        myLogState.Clear();
                        Console.WriteLine("restart log");
                        // TODO event?
                    }
                    StationProtocolVersion = protocol_version;
                    stationContestID = contest_ID;
                }

                if (wtlogsyncState == WTLOGSYNCSTATE.WAIT_HELLO)
                    wtlogsyncState = WTLOGSYNCSTATE.HELLO_RECEIVED;

                StationSyncStatus w = new StationSyncStatus(e.Msg.Src, first_QSO_ts);
                int sl_index = wtStationSyncList.FindLastIndex(x => x.from == e.Msg.Src);
                if (sl_index != -1)
                {
                    wtStationSyncList[sl_index].timestamp = w.timestamp;
                    wtStationSyncList[sl_index].first_QSO_ts = w.first_QSO_ts;
                }
                else
                {
                    wtStationSyncList.Add(w);
                }
            }
            else
            // we need to parse STATUS, too, as HELLO is only sent when a new log is opened...
            if (e.Msg.Msg == WTMESSAGES.STATUS && e.Msg.HasChecksum)
            {
                string[] data = e.Msg.Data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                int sl_index = wtStationSyncList.FindLastIndex(x => x.from == e.Msg.Src);
                // use 0 for unknown parts - we are mainly interested in the name
                StationSyncStatus w = new StationSyncStatus(e.Msg.Src, DateTime.MinValue);
                if (sl_index != -1)
                {
                    wtStationSyncList[sl_index].timestamp = w.timestamp;
                }
                else
                {
                    wtStationSyncList.Add(w);
                }

                if (wtlogsyncState == WTLOGSYNCSTATE.WAIT_HELLO)
                    wtlogsyncState = WTLOGSYNCSTATE.HELLO_RECEIVED;
            }
            else
                if (e.Msg.Msg == WTMESSAGES.IHAVE && e.Msg.HasChecksum)
            {
                /*
                    # IHAVE protocol (log synchronization)
                    #   "StationName" "ToStation" "StationUniqueID" "AvailableFrom" <Unique ID count>
                    #     "AvailableFrom" can be one of "OWNER" (O), "LOGGEDON", "LOGGEDELSE" (E)
                    # 'IHAVE: "Office" "" "Shack@82"   "LOGGEDELSE" 47\xfd\x00'
                    # 'IHAVE: "Office" "" "Shack@83"   "LOGGEDELSE" 60\xf9\x00'
                    # 'IHAVE: "Office" "" "Office@169" "OWNER"      2\x8c\x00'    <-- 1.28
                    # 'IHAVE: "Office" "" "Office@154" O            1 1 3\xe5\x00'  <-- 1.28
                    # 'IHAVE: "Shack"  "" "Shack@9"    E            1 1 911-1-117\x86\x00' <-- 1.29
                    # 'IHAVE:              micro@57540 E            1 1 30-1-3   <-- 1..30, 1 missing, 3 total, so 32-33
                    # 0       1        2  3            4            5 6 7

                  "StationName" "ToStation" "StationUniqueID" Origin <FirstRow> <InitialState> "Run-Lengths"
                  Origin can be O (Owner or LoggedOn) or E (LoggedElse).
                  The inventory is composed of : FirstRow (starting at 1), InitialState (0 or 1)
                  and Hyphen-separated Run Lengths

                   - The IHAVE: frames (and IHAVEWAN: frames for the BridgeHead) contain an
                     inventory / status of all the QSO, for a given "StationName@wUniqueLogID" key.
                     To keep these inventories short, a Run Length Encoding (RLE) is performed,
                     and inventories are split if necessary.

                     <InitialState> can only be 1 (=exists) or 0 (="not in my log")
                      
                  Inventory string example :

                  "100 1 10-5-5" means : First Q index of this inventory string = 100 - The Q of the first index is
                  existing (1) and there are 10 of them (from 100 to 109), the next 5 are not existing = missing
                  (from 110 to 114), and the next 5 are existing (from 115 to 119)

                */
                string[] data = e.Msg.Data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (data.Length == 5 && (e.Msg.Dst == "" || e.Msg.Dst == my_wtname))
                {
                    string stationUniqueID = "";
                    try
                    {
                        stationUniqueID = data[0];
                    }
                    catch
                    {
                    }
                    StationLogStat.wtAvailableFrom AvailableFrom;

                    if (data[1] == "E" || data[1] == "LOGGEDELSE")
                        AvailableFrom = StationLogStat.wtAvailableFrom.LoggedElse;
                    else
                        AvailableFrom = StationLogStat.wtAvailableFrom.Owner;

                    uint FirstRow;
                    if (!UInt32.TryParse(data[2], out FirstRow))
                        FirstRow = 0;
                    uint InitialState;
                    if (!UInt32.TryParse(data[3], out InitialState))
                        InitialState = 0;

                    // can be of the format "30-1-3" - run length encoded
                    string[] count_to_data = data[4].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                    List<logSegment> ls = new List<logSegment>();

                    uint rle_index = 0;
                    int segment = 0;
                    ls.Add(new logSegment());

                    if (InitialState == 0)
                    {
                        // we do not have the first QSO(s)
                        // so first one is already run-length encoded
                        if (count_to_data.Length % 2 == 1 || count_to_data.Length < 2)
                            return;
                        uint count_from, count_diff;
                        if (!UInt32.TryParse(count_to_data[0], out count_from))
                            return;
                        if (!UInt32.TryParse(count_to_data[1], out count_diff))
                            return;
                        ls[0].count_from = count_from + 1;
                        ls[0].count_to = count_from + count_diff;
                        rle_index = 2;
                    }
                    else
                    {
                        // if we start at QSO index 1, RLE must be an odd number
                        if (count_to_data.Length % 2 == 0)
                            return;
                        uint count_to;
                        if (!UInt32.TryParse(count_to_data[0], out count_to))
                            return;

                        ls[0].count_from = 1;
                        ls[0].count_to = count_to;
                        rle_index = 1;
                    }

                    while(rle_index < count_to_data.Length)
                    {
                        segment++;
                        ls.Add(new logSegment());
                        uint count_skip, count_diff;
                        if (!UInt32.TryParse(count_to_data[rle_index], out count_skip))
                            return;
                        if (!UInt32.TryParse(count_to_data[rle_index+1], out count_diff))
                            return;
                        ls[segment].count_from = ls[segment-1].count_to + count_skip + 1;
                        ls[segment].count_to = ls[segment - 1].count_to + count_skip + count_diff;
                        rle_index += 2;
                    }

//                    Console.WriteLine("IHAVE: from " + e.Msg.Src + " stationUniqueID " + stationUniqueID + " AvailableFrom " + AvailableFrom.ToString() +
//                        " uniqueIDCount " + FirstRow + " QSOs " + count_from + "-" + count_to + " and " + count_next + "-" + count_end);

                    StationLogStat sls = new StationLogStat(stationUniqueID, AvailableFrom, ls);

                    int sl_index = wtStationSyncList.FindLastIndex(x => x.from == e.Msg.Src);
                    if (sl_index != -1)
                    {
                        int ssl_index = wtStationSyncList[sl_index].logstat.FindLastIndex(x => x.StationUniqueID == stationUniqueID);
                         if (ssl_index != -1)
                        {
                            if (wtStationSyncList[sl_index].logstat[ssl_index] != sls)
                                wtStationSyncList[sl_index].logstat[ssl_index] = sls;
                        }
                        else
                        {
                            wtStationSyncList[sl_index].logstat.Add(sls);
                        }

//                        foreach (var s in sl.logstat)
//                            Console.WriteLine(sl.from + ": " + s.StationUniqueID + " " + s.count_from + "-" + s.count_to);
                    }

                }
            }
            else
            if (e.Msg.Msg == WTMESSAGES.ADDQSO && e.Msg.HasChecksum)
            {
                /*
                    #
                    # ADDQSO: "FromStation" "ToStation" "StationName" <time> <freq> <modeID> <bandID>
                    #   <Radio> <RunStn> <StationFlags> <dwUniqueID> <SerialNum> "LoggedCall" "RprtSend"
                    #   "RprtRcvd" "GridSquare" "MiscInfo" "MiscInfo2" <QtcSerialSent> "FromCounty"
                    #   "Precedence" "Operator" <LogUniqueID>

                    # 'ADDQSO: "Shack" "Office" "Shack" 1409117612 140000 0 5
                    # {0       1       2        3       4          5      6 7}
                    #   0 0 0  1  1  "N1AA" ""
                    #  {8 9 10 11 12 13     14}
                    #   "CT" "" "ED" "2" 0 ""
                    #   {15  16 17   18 19 20}
                    #   "" "" 75\x91\x00'
                    #  {21 22 23}
                */
                string[] data = e.Msg.Data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                string StationName = data[0];

                uint txIDQSONumber;
                if (!UInt32.TryParse(data[8], out txIDQSONumber))
                    txIDQSONumber = 0;

                // Sometimes for 1 QSO the txID number is 0 instead of 1 for first QSO
                if (txIDQSONumber == 0)
                    txIDQSONumber = 1;

                DataRow row = QSO.NewRow();
                row["CALL"] = data[10];

                string band;
                try
                {
                    band = Enum.Parse(typeof(WTBANDS), data[4]).ToString().Replace("Band", "").Replace("Hz", "");
                }
                catch
                {
                    band = ""; // FIXME
                }
                row["BAND"] = band;

                DateTime ts = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                ts = ts.AddSeconds((double)Int32.Parse(data[1]));
                row["TIME"] = ts.ToString("HH:mm"); // FIXME warum nicht als object? So fehlt das Datum
                uint serial_sent = UInt32.Parse(data[9]);
                row["SENT"] = data[11] + serial_sent.ToString("D3");
                row["RCVD"] = data[12];
                row["LOC"] = data[13];

                row["RUNSTN"] = StationName;
                if (data.Length == 17)
                    row["LOGID"] = data[16];
                else
                    row["LOGID"] = data[15];
                row["LOGNR"] = txIDQSONumber;

#if DEBUG_PACKET_LOSS
                if (skip_paket())
                    return;
#endif
                // If broadcast ADDQSO, or directed only to me from a NEEDQSO request, add it to my ds
                if (e.Msg.Dst == "" || e.Msg.Dst == my_wtname)
                {
                    try
                    {
                        var qso = QSO.Rows.Find(new string[]
                        {
                                row["RUNSTN"].ToString(),
                                row["LOGID"].ToString(),
                                row["LOGNR"].ToString()
                        });

                        if (qso != null)
                        {
                            if (!(qso.ItemArray.SequenceEqual(row.ItemArray)))
                            {
                                qso.Delete();
                                QSO.Rows.Add(row);
                            }
                        }
                        else
                        {
                            QSO.Rows.Add(row);
                        }

                        string StationUniqueID = StationName + "@" + row["LOGID"].ToString();
                        var myls = myLogState.Find(x => x.StationUniqueID.Equals(StationUniqueID));

                        if (myls != null)
                        {

                            if (txIDQSONumber == myls.ls.Last().count_to + 1)
                            {
                                myls.ls.Last().count_to = txIDQSONumber;
                                // Console.WriteLine("we have2 " + myls.StationUniqueID + ":" + myls.count_from + "-" + myls.count_to + "-" + myls.count_next + "-" + myls.count_end);
                            }
                            else
                            {
                                find_qsos_count_to(ref myls);
                                //Console.WriteLine("we have " + myls.StationUniqueID + ":" + myls.count_from + "-" + myls.count_to + "-" + myls.count_next + "-" + myls.count_end);
                            }
                        }
                        else
                        {
                            logState lst = new logState(StationUniqueID, new List<logSegment>());
                            find_qsos_count_to(ref lst);
                            //Console.WriteLine("we have now " + ls.StationUniqueID + ":" + ls.count_from + "-" + ls.count_to + "-" + ls.count_next + "-" + ls.count_end);
                            myLogState.Add(lst);
                        }

                        if (needQSOsentForStationID.Equals(StationUniqueID) && (needQSOsentForCountTo == txIDQSONumber))
                        {
                            // received everything from last "NEEDQSO"
                            Console.WriteLine("next needqso");
                            ti_get_log.Stop();
                            ti_get_log.Interval = 200; // quick restart
                            ti_get_log.Start();
                        }
                    }
                    catch(Exception ex) { Console.WriteLine(ex.Message);  }
                }
            }
            else if (e.Msg.Msg == WTMESSAGES.UPDQSO && e.Msg.HasChecksum)
            {
                /*
                    # UPDQSO: "FromStation" "ToStation"   
                    #   "OrigStationName" <oldtime> <oldfreq> <oldmodeID> "OldLoggedCall" "OldStationName"
                    #   <freq> <modeID> <Radio> <RunStn> <StationFlags> <dwUniqueID> <SerialNum> "LoggedCall" "RprtSend"
                    #   "RprtRcvd" "GridSquare" "MiscInfo" "MiscInfo2" <QtcSerialSent> "FromCounty"
                    #   "Precedence" "Operator" <LogUniqueID>
                    #
                    # 'UPDQSO: "10GHz" "" "10GHz" 1650212808 12960000 1 "DG3AAD" "CHECK" 1650212808 12960000 1 0 0 0 1 146 "DG3AAD/P" "59" "59004" "JO41WM" "" "" 0 "" "" "" 63986
                    #
                 */
                string[] data = e.Msg.Data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                string StationName = data[0];
                string OldStationName = data[5];

                uint txIDQSONumber;
                if (!UInt32.TryParse(data[12], out txIDQSONumber))
                    txIDQSONumber = 0;

                // Sometimes for 1 QSO the txID number is 0 instead of 1 for first QSO
                if (txIDQSONumber == 0)
                    txIDQSONumber = 1;

                string logid = data[19];
#if DEBUG_PACKET_LOSS
                if (skip_paket())
                    return;
#endif
                // If broadcast ADDQSO, or directed only to me from a NEEDQSO request, add it to my ds
                if (e.Msg.Dst == "" || e.Msg.Dst == my_wtname)
                {
                    try
                    {
                        var qso = QSO.Rows.Find(new string[]
                        {
                                OldStationName,
                                logid,
                                txIDQSONumber.ToString()
                        });

                        if (qso != null)
                        {
                            //Console.WriteLine("was " + qso["CALL"].ToString() + " " + qso["TIME"].ToString() + " " + qso["SENT"].ToString() + " " + qso["RCVD"].ToString() + " " + qso["LOC"].ToString());
                            qso["CALL"] = data[14];

                            DateTime ts = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            ts = ts.AddSeconds((double)Int32.Parse(data[6]));
                            qso["TIME"] = ts.ToString("HH:mm"); // FIXME warum nicht als object? So fehlt das Datum
                            uint serial_sent = UInt32.Parse(data[13]);
                            qso["SENT"] = data[15] + serial_sent.ToString("D3");
                            qso["RCVD"] = data[16];
                            qso["LOC"] = data[17];
                            //Console.WriteLine("now " + qso["CALL"].ToString() + " " + qso["TIME"].ToString() + " " + qso["SENT"].ToString() + " " + qso["RCVD"].ToString() + " " + qso["LOC"].ToString());
                        }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }

            }
        }
    }
}