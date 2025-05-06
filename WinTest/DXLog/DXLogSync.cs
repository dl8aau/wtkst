//#define DEBUG_PACKET_LOSS

using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using WinTest.DXLog;

namespace WinTest.DXL
{
    /*
     <?xml version="1.0"?>
     <contactinfo>
       	<logger>DXLog.net v2.6.9</logger>
       	<qsoid>2</qsoid>
       	<contestname>EUROPEAN-VHF</contestname>
       	<timestamp>2024-10-16 08:45:30</timestamp>
       	<mycall>M1DST</mycall>
       	<band>144</band>
       	<txfreq>14420000</txfreq>
       	<operator/>
       	<mode>CW</mode>
       	<call>G4CLA</call>
       	<countryprefix>G</countryprefix>
       	<wpxprefix>G4</wpxprefix>
       	<snt>599</snt>
       	<rcv>599</rcv>
       	<nr>1</nr>
       	<exch1>282</exch1>
       	<exch2>IO92JL</exch2>
       	<exch3/>
       	<exch4/>
       	<xqso>False</xqso>
       	<invalid>False</invalid>
       	<duplicate>False</duplicate>
       	<rule10broken>False</rule10broken>
       	<azimuth>11</azimuth>
       	<distance>138</distance>
       	<stationid>STN1</stationid>
       	<stationqso>2</stationqso>
       	<stationtype>R</stationtype>
       	<local>True</local>
       	<mult1>IO92</mult1>
       	<mult2/>
       	<mult3/>
       	<points>112</points>
       	<period>1</period>
       	<guid>53d3c3461ce1483bb9e4311d3eecc184</guid>
       	<newqso>False</newqso>
    </contactinfo>
    */


    [XmlRootAttribute("contactinfo", IsNullable = false)]
    public class ContactInfo
    {
        public string logger;
        public string contestname;

        [XmlIgnore]
        public DateTime timestamp { get; set; }

        [XmlElement("timestamp")]
        public string timestampString
        {
            get => timestamp.ToString("yyyy-MM-dd HH:mm:ss");
            set => timestamp = DateTime.Parse(value);
        }

        public string mycall;
        public string band;
        public int txfreq;
        [XmlElement(ElementName = "operator")]
        public string op;
        public string mode;
        public string call;
        public string countryprefix;
        public string wpxprefix;
        public string snt;
        public string rcv;
        public string nr;
        public string exch1;
        public string exch2;
        public string exch3;
        public string exch4;

        [XmlIgnore]
        public bool duplicate { get; set; }

        [XmlIgnore]
        public bool invalid { get; set; }

        [XmlIgnore]
        public bool rule10broken { get; set; }

        [XmlIgnore]
        public bool local { get; set; }

        [XmlIgnore]
        public bool newqso { get; set; }

        [XmlElement("duplicate")]
        public string duplicateString
        {
            get => duplicate.ToString();
            set => duplicate = bool.Parse(value);
        }

        [XmlElement("stationid")]
        public string stationname;

        public string stationtype;
        public string mult1;
        public string mult2;
        public string mult3;
        public Guid guid;

        public int points;
        public int qsoid;
        public int azimuth;
        public int distance;
        public int stationqso;
        public int period;
    }

    public class DXLogListener
    {
        private volatile bool listen;
        private UdpClient udpClient;

        private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private void serializer_UnknownAttribute
        (object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Console.WriteLine("Unknown attribute " +
            attr.Name + "='" + attr.Value + "'");
        }

        public DXLogListener(int udpPort)
        {
            var ep = new IPEndPoint(IPAddress.Any, udpPort);
            udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            udpClient.Client.Bind(ep);
            listen = true;

            var listener = new Thread(() =>
            {
                while (listen)
                {
                    try
                    {
                        byte[] data = udpClient.Receive(ref ep);
                        if (data.Length > 0)
                        {
                            var ustream = new System.IO.MemoryStream(data);

                            XmlSerializer serializer = new XmlSerializer(typeof(ContactInfo));
                            /* If the XML document has been altered with unknown
                            nodes or attributes, handle them with the
                            UnknownNode and UnknownAttribute events.*/
                            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
                            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

                            ContactInfo ci;
                            /* Use the Deserialize method to restore the object's state with
                            data from the XML document. */
                            ci = (ContactInfo)serializer.Deserialize(ustream);
                            // Read the order date.
                            Console.WriteLine("call: " + ci.call);

                            //Console.WriteLine("WT Msg " + Msg.Msg + " src " + Msg.Src + " dest " + Msg.Dst + " data " + Msg.Data + " checksum " + Msg.HasChecksum);
                            DXLogMessageReceived?.Invoke(this, new DXLogMessageEventArgs(ci));
                        }
                    }
                    catch (SocketException)
                    {
                        break; // exit the while loop
                    }
                }
                //Console.WriteLine("listener done");
            });
            listener.IsBackground = true;
            listener.Start();
        }

        public event EventHandler<DXLogMessageEventArgs> DXLogMessageReceived;
        public class DXLogMessageEventArgs : EventArgs
        {
            /// <summary>
            /// Win-Test message
            /// </summary>
            public DXLogMessageEventArgs(ContactInfo ci)
            {
                this.ci = ci;
            }
            public ContactInfo ci { get; private set; }
        }

        public void close()
        {
            listen = false;

            udpClient.Client.Shutdown(SocketShutdown.Both);
            udpClient.Close();
        }
    }

    public class DXLogSync : WinTestLogBase
    {
        private DXLogListener qtl;
        private DXLogNetworkClient _networkClient;

        public DXLogSync(LogWriteMessageDelegate mylog) : base(mylog)
        {

            QSO.Columns.Add("TXNR");
            DataColumn[] QSOkeys = new DataColumn[]
            {
                QSO.Columns["TXNR"],
                QSO.Columns["BAND"]
            };
            QSO.PrimaryKey = QSOkeys;
        }

        public override void Dispose()
        {
            if (qtl != null)
                qtl.close();

            _networkClient?.StopListening();
        }


        public override string getStatus()
        {
            return QSO.Rows.Count.ToString();
        }

        public override void Get_QSOs(string wtname)
        {
            // start listener
            if (qtl == null)
            {
                QSO.Clear();
                qtl = new DXLogListener(12060); // DXLog default UDP port
                qtl.DXLogMessageReceived += DXLogMessageReceivedHandler;
            }

            if (_networkClient == null)
            {
                _networkClient = new DXLogNetworkClient();
                _networkClient.DXLogQsoReceived += _networkClient_DXLogQsoReceived;
                _networkClient.StationJoinedNetwork += _networkClient_StationJoinedNetwork; ;
                _networkClient.StationLeftNetwork += _networkClient_StationLeftNetwork;
                _ = _networkClient.StartListeningAsync();
            }

        }

        private void _networkClient_StationLeftNetwork(object sender, string e)
        {
            Console.WriteLine($"DXLog client left : {e}");
        }

        private void _networkClient_StationJoinedNetwork(object sender, NetworkMsgStatusInfo e)
        {
            Console.WriteLine($"DXLog client joined : {e.StationID} on {e.Band}");
        }

        /// <summary>
        /// This is fired every time a QSO is received as part of a sync request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _networkClient_DXLogQsoReceived(object sender, DXQSO e)
        {
            // QSO ContactInfo received
            var row = QSO.NewRow();
            row["CALL"] = e.Callsign;

            switch (e.Band)
            {
                case "50":
                    row["BAND"] = "50M";
                    break;
                case "70":
                    row["BAND"] = "70M";
                    break;
                case "144":
                    row["BAND"] = "144M";
                    break;
                case "432":
                    row["BAND"] = "432M";
                    break;
                case "1296":
                    row["BAND"] = "1_2G";
                    break;
                case "2320":
                    row["BAND"] = "2_3G";
                    break;
                case "3400":
                    row["BAND"] = "3_4G";
                    break;
                case "5700":
                    row["BAND"] = "5_7G";
                    break;
                case "10G":
                    row["BAND"] = "10G";
                    break;
                case "24G":
                    row["BAND"] = "24G";
                    break;
                case "47G":
                    row["BAND"] = "47G";
                    break;
                case "76G":
                    row["BAND"] = "76G";
                    break;
                default:
                    row["BAND"] = "";
                    break;
            }

            row["TIME"] = e.TimeStamp.ToString("HH:mm");
            row["TXNR"] = e.Nr;
            row["SENT"] = $"{e.Sent} {e.Nr}";
            row["RCVD"] = $"{e.Rcvd}";
            row["LOC"] = e.RecInfo;

            try
            {
                var key = new object[] { row["TXNR"], row["BAND"] };
                var qso = QSO.Rows.Find(key);
                if (qso != null)
                {
                    // Update only if necessary fields differ
                    if (!qso["CALL"].Equals(row["CALL"]) || !qso["TIME"].Equals(row["TIME"]) || !qso["SENT"].Equals(row["SENT"]) || !qso["RCVD"].Equals(row["RCVD"]) || !qso["LOC"].Equals(row["LOC"]))
                    {
                        qso["CALL"] = row["CALL"];
                        qso["TIME"] = row["TIME"];
                        qso["SENT"] = row["SENT"];
                        qso["RCVD"] = row["RCVD"];
                        qso["LOC"] = row["LOC"];
                    }
                }
                else
                {
                    QSO.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing QSO: {ex.Message}\n{ex.StackTrace}");
            }

        }

        /// <summary>
        /// This is fired due to a normal QSO broadcast.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DXLogMessageReceivedHandler(object sender, DXLogListener.DXLogMessageEventArgs e)
        {
            // QSO ContactInfo received
            DataRow row = QSO.NewRow();
            row["CALL"] = e.ci.call;

            switch (e.ci.band)
            {
                case "50":
                    row["BAND"] = "50M";
                    break;
                case "70":
                    row["BAND"] = "70M";
                    break;
                case "144":
                    row["BAND"] = "144M";
                    break;
                case "432":
                    row["BAND"] = "432M";
                    break;
                case "1296":
                    row["BAND"] = "1_2G";
                    break;
                case "2320":
                    row["BAND"] = "2_3G";
                    break;
                case "3400":
                    row["BAND"] = "3_4G";
                    break;
                case "5700":
                    row["BAND"] = "5_7G";
                    break;
                case "10G":
                    row["BAND"] = "10G";
                    break;
                case "24G":
                    row["BAND"] = "24G";
                    break;
                case "47G":
                    row["BAND"] = "47G";
                    break;
                case "76G":
                    row["BAND"] = "76G";
                    break;
                default:
                    row["BAND"] = "";
                    break;
            }

            row["TIME"] = e.ci.timestamp.ToString("HH:mm"); // FIXME warum nicht als object? So fehlt das Datum
            uint serial_sent = UInt32.Parse(e.ci.nr);
            row["TXNR"] = e.ci.nr;
            row["SENT"] = e.ci.snt + serial_sent.ToString("D3");
            row["RCVD"] = e.ci.rcv + e.ci.exch1;
            row["LOC"] = e.ci.exch2;
            try
            {
                var qso = QSO.Rows.Find(new object[]
                {
                                row["TXNR"],
                                row["BAND"]
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

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }

}