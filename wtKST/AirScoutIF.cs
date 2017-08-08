using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WinTest;
using wtKST.Properties;

namespace wtKST
{
    class AirScoutInterface
    {
        public Dictionary<string, PlaneInfoList> planes = new Dictionary<string, PlaneInfoList>();

        public void send_watchlist(string watchlist, string MyCall, string MyLoc)
        {
            string qrg = qrg_from_settings();
            wtMessage Msg = new wtMessage(WTMESSAGES.ASWATCHLIST, Settings.Default.AS_My_Name, Settings.Default.AS_Server_Name, string.Concat(new string[]
            {
                qrg, ",", WCCheck.WCCheck.Cut(MyCall), ",", MyLoc, watchlist
            }));
            try
            {
                UdpClient client = new UdpClient();
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                client.Client.ReceiveTimeout = 10000;
                IPEndPoint groupEp = new IPEndPoint(IPAddress.Broadcast, Settings.Default.AS_Port);
                client.Connect(groupEp);
                byte[] b = Msg.ToBytes();
                client.Send(b, b.Length);
                client.Close();
            }
            catch
            {
            }
        }

        public string qrg_from_settings()
        {
            string qrg = "1440000";
            if (Settings.Default.AS_QRG == "432M")
            {
                qrg = "4320000";
            }
            else if (Settings.Default.AS_QRG == "1.2G")
            {
                qrg = "12960000";
            }
            else if (Settings.Default.AS_QRG == "2.3G")
            {
                qrg = "23200000";
            }
            else if (Settings.Default.AS_QRG == "3.4G")
            {
                qrg = "34000000";
            }
            else if (Settings.Default.AS_QRG == "5.7G")
            {
                qrg = "57600000";
            }
            else if (Settings.Default.AS_QRG == "10G")
            {
                qrg = "103680000";
            }
            return qrg;
        }

        /* called from BackgroundWorker bw_GetPlanes - reports results through bw_GetPlanes.ReportProgress */
        public bool GetPlanes(string mycall, string myloc, string dxcall, string dxloc, ref BackgroundWorker bw_GetPlanes)
        {

            string rxmycall = "";
            string rxdxcall = "";
            string qrg = qrg_from_settings();

            wtMessage Msg = new wtMessage(WTMESSAGES.ASSETPATH, Settings.Default.AS_My_Name, Settings.Default.AS_Server_Name, 
                string.Concat(new string[] { qrg, ",", mycall, ",", myloc, ",", dxcall, ",", dxloc }));
            try
            {
                UdpClient client = new UdpClient();
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                client.Client.ReceiveTimeout = 10000;
                IPEndPoint groupEp = new IPEndPoint(IPAddress.Broadcast, Settings.Default.AS_Port);
                client.Connect(groupEp);
                byte[] b = Msg.ToBytes();
                client.Send(b, b.Length);
                client.Close();
                DateTime start = DateTime.UtcNow;
                int rcvtimeout = 30;
                try
                {
                    rcvtimeout = Convert.ToInt32(Settings.Default.AS_Timeout);
                }
                catch
                {
                    rcvtimeout = 30;
                }
                int elapsed;
                bool match;
                do
                {
                    IPEndPoint ep = new IPEndPoint(IPAddress.Any, Settings.Default.AS_Port);
                    UdpClient u = new UdpClient();
                    u.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                    u.Client.ReceiveTimeout = 1000;
                    u.Client.Bind(ep);
                    try
                    {
                        byte[] data = u.Receive(ref ep);
                        if (data.Length > 0)
                        {
                            string text = Encoding.ASCII.GetString(data);
                            text = text.Remove(text.Length - 1);
                            byte sum = 0;
                            for (int j = 0; j < data.Length - 1; j++)
                            {
                                sum += data[j];
                            }
                            sum |= 128;
                            Console.WriteLine(string.Concat(new object[]
                            {
                                ep.Address,
                                " >> ",
                                text,
                                "[",
                                data[data.Length - 1].ToString("X2"),
                                "<>",
                                sum.ToString("X2"),
                                "]"
                            }));
                            Msg = new wtMessage(data);
                            if (Msg.Msg == WTMESSAGES.ASNEAREST)
                            {
                                string[] a = Msg.Data.Split(new char[] { ',' });
                                rxmycall = a[1];
                                rxdxcall = a[3];
                            }
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (u != null)
                        {
                            u.Close();
                        }
                    }
                    elapsed = (DateTime.UtcNow - start).Seconds;
                    match = (Msg.Dst == Settings.Default.AS_My_Name && Msg.Msg == WTMESSAGES.ASNEAREST 
                        && mycall == rxmycall && dxcall == rxdxcall);
                    if (match)
                    {
                        bw_GetPlanes.ReportProgress(1, Msg);
                    }
                    Thread.Sleep(100);
                } while (!match && elapsed < rcvtimeout);

                if (elapsed >= rcvtimeout)
                {
                    bw_GetPlanes.ReportProgress(0, dxcall);
                    return false;
                }
                return true;
            }
            catch (Exception e1_596)
            {
                bw_GetPlanes.ReportProgress(0, null);
                bw_GetPlanes.ReportProgress(-1, Msg);
                return false;
            }
        }

        public bool process_msg_asnearest(wtMessage Msg, out string dxcall)
        {
            dxcall = "";
            if (Msg.Msg == WTMESSAGES.ASNEAREST)
            {
                try
                {
                    PlaneInfoList infolist = new PlaneInfoList();
                    string[] a = Msg.Data.Split(new char[] { ',' });
                    DateTime utc = Convert.ToDateTime(a[0]).ToUniversalTime();
                    string mycall = a[1];
                    string myloc = a[2];
                           dxcall = a[3];
                    string dxloc = a[4];
                    int planecount = Convert.ToInt32(a[5]);
                    infolist.UTC = utc;
                    for (int i = 0; i < planecount; i++)
                    {
                        PlaneInfo info = new PlaneInfo(a[6 + i * 5], a[7 + i * 5], Convert.ToInt32(a[8 + i * 5]), Convert.ToInt32(a[9 + i * 5]), Convert.ToInt32(a[10 + i * 5]));
                        infolist.Add(info);
                    }
                    planes.Remove(dxcall);
                    if (infolist.Count > 0)
                    {
                        infolist.Sort(new PlaneInfoComparer());
                        planes.Add(dxcall, infolist);
                        return true;
                    }
                }
                catch (Exception e1_211)
                {
                }
            }
            return false;
        }

        public void show_path(string call, string loc, string MyCall, string MyLoc)
        {
            string qrg = qrg_from_settings();
            wtMessage Msg = new wtMessage(WTMESSAGES.ASSHOWPATH, Settings.Default.AS_My_Name, Settings.Default.AS_Server_Name, 
                string.Concat(new string[] { qrg, ",",  WCCheck.WCCheck.Cut(MyCall), ",",  MyLoc, ",",
                    WCCheck.WCCheck.Cut(call), ",", loc }));
            UdpClient client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            client.Client.ReceiveTimeout = 10000;
            IPEndPoint groupEp = new IPEndPoint(IPAddress.Broadcast, Settings.Default.AS_Port);
            client.Connect(groupEp);
            byte[] b = Msg.ToBytes();
            client.Send(b, b.Length);
            client.Close();
        }

        public void process_msg_setpath(wtMessage Msg)
        {
            if (Msg.Msg == WTMESSAGES.ASSETPATH)
            {
                try
                {
                    string[] a = Msg.Data.Split(new char[] { ',' });
                    string mycall = a[0];
                    string myloc = a[1];
                    string dxcall = a[2];
                    string dxloc = a[3];
                    planes.Remove(dxcall);
                }
                catch (Exception e1_211)
                {
                }
            }
        }

        public string GetNearestPlanes(string call)
        {
            PlaneInfoList infolist = null;
            string result;
            if (planes.TryGetValue(call, out infolist))
            {
                string s = DateTime.UtcNow.ToString("HH:mm") + " [" + (DateTime.UtcNow - infolist.UTC).Minutes.ToString() + "mins ago]\n\n";
                foreach (PlaneInfo info in infolist)
                {
                    s = string.Concat(new object[]
                    {
                        s,
                        info.Potential.ToString(),
                        " : ",
                        info.Call,
                        "[",
                        info.Category,
                        "] --> ",
                        info.IntQRB.ToString(),
                        "km [",
                        info.Mins,
                        "mins]\n"
                    });
                }
                result = s;
            }
            else
            {
                result = "";
            }
            return result;
        }

        public string GetNearestPlanePotential(string call)
        {
            call = call.TrimStart(new char[] { '(' }).TrimEnd(new char[] { ')' });
            PlaneInfoList infolist = null;
            string result;
            if (planes.TryGetValue(call, out infolist))
            {
                result = infolist[0].Potential + "," + infolist[0].Category;
            }
            else
            {
                result = "0";
            }
            return result;
        }

    }
}
