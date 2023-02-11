//#define DEBUG_PACKET_LOSS

using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace WinTest
{
    /*
     * <?xml version="1.0"?
     * ><contactinfo>
     * <logger>QARTest 12.2.1</logger>
     * <contestname></contestname>
     * <timestamp>2022-04-23 09:14:41</timestamp>
     * <mycall>DL8AAU</mycall>
     * <band>432</band>
     * <txfreq>0</txfreq>
     * <operator></operator>
     * <mode>USB</mode>
     * <call>DL0TD</call>
     * <countryprefix>DL</countryprefix>
     * <wpxprefix>DL0</wpxprefix>
     * <snt>59</snt>
     * <rcv>59</rcv>
     * <nr>2</nr>
     * <exch1>044</exch1>
     * <exch2>JN49HV</exch2>
     * <exch3></exch3>
     * <duplicate>False</duplicate>
     * <stationname>QUARTAAU</stationname>
     * <points>6</points>
     * </contactinfo>
    */
    [XmlRootAttribute("contactinfo",
    IsNullable = false)]


    public class contactinfo
    {
        public string logger;
        public string contestname;
        [XmlIgnore]
        public DateTime timestamp { get; set; }

        [XmlElement("timestamp")]
        public string timestampString
        {
            get { return this.timestamp.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { this.timestamp = DateTime.Parse(value); }
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
        [XmlIgnore]
        public bool duplicate { get; set; }
        [XmlElement("duplicate")]
        public string duplicateString
        {
            get { return this.duplicate.ToString(); }
            set { this.duplicate = Boolean.Parse(value); }
        }
        public string stationname;
        public int points;
    }
    public class QARTtestListener
    {
        private volatile bool Listen;
        private UdpClient u;

        private void serializer_UnknownNode
(object sender, XmlNodeEventArgs e)
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

        public QARTtestListener(int UDPPort)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, UDPPort);
            u = new UdpClient();
            u.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            u.Client.Bind(ep);
            Listen = true;

            Thread listener = new Thread(() =>
            {
                while (Listen == true)
                {
                    try
                    {
                        byte[] data = u.Receive(ref ep);
                        if (data.Length > 0)
                        {
                            var ustream = new System.IO.MemoryStream(data);

                            XmlSerializer serializer = new XmlSerializer(typeof(contactinfo));
                            /* If the XML document has been altered with unknown
                            nodes or attributes, handle them with the
                            UnknownNode and UnknownAttribute events.*/
                            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
                            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

                            contactinfo ci;
                            /* Use the Deserialize method to restore the object's state with
                            data from the XML document. */
                            ci = (contactinfo)serializer.Deserialize(ustream);
                            // Read the order date.
                            Console.WriteLine("call: " + ci.call);

                            //Console.WriteLine("WT Msg " + Msg.Msg + " src " + Msg.Src + " dest " + Msg.Dst + " data " + Msg.Data + " checksum " + Msg.HasChecksum);
                            if (QTMessageReceived != null)
                            {
                                QTMessageReceived(this, new QTMessageEventArgs(ci));
                            }
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

        public event EventHandler<QTMessageEventArgs> QTMessageReceived;
        public class QTMessageEventArgs : EventArgs
        {
            /// <summary>
            /// Win-Test message
            /// </summary>
            public QTMessageEventArgs(contactinfo ci)
            {
                this.ci = ci;
            }
            public contactinfo ci { get; private set; }
        }

        public void close()
        {
            Listen = false;

            u.Client.Shutdown(SocketShutdown.Both);
            u.Close();
        }
    }

    public class QARTestLogSync : WinTestLogBase
    {
        private QARTtestListener qtl;

        public QARTestLogSync(LogWriteMessageDelegate mylog) : base(mylog)
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
                qtl = new QARTtestListener(9458); // QARTest default UDP port
                qtl.QTMessageReceived += QTMessageReceivedHandler;
            }
        }

        private void QTMessageReceivedHandler(object sender, QARTtestListener.QTMessageEventArgs e)
        {
            // QSO contactinfo received
            DataRow row = QSO.NewRow();
            row["CALL"] = e.ci.call;

            switch (e.ci.band)
            {
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