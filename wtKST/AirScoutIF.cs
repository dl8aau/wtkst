using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
        private IPEndPoint localep;
        private WinTest.wtListener wtl;
        private BackgroundWorker bw_GetPlanes;

        private string mycall;
        private string dxcall;
        private EventWaitHandle waitHandle;

        public AirScoutInterface(ref BackgroundWorker bw_GetPlanes)
        {
            localep = new IPEndPoint(GetIpIFDefaultGateway(), 0);

            this.bw_GetPlanes = bw_GetPlanes;
            waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

            wtl = new WinTest.wtListener(Settings.Default.AS_Port);
            wtl.wtMessageReceived += wtMessageReceivedHandler;
        }

        private void wtMessageReceivedHandler(object sender, WinTest.wtListener.wtMessageEventArgs e)
        {
            if (e.Msg.Msg == WTMESSAGES.ASNEAREST && e.Msg.HasChecksum)
            {
                string[] a = e.Msg.Data.Split(new char[] { ',' });
                var rxmycall = a[1];
                var rxdxcall = a[3];

                if (e.Msg.Dst == Settings.Default.AS_My_Name && e.Msg.Msg == WTMESSAGES.ASNEAREST
                    && mycall == rxmycall)
                {
                    if (process_msg_asnearest(e.Msg))
                    {
                        bw_GetPlanes.ReportProgress(1, rxdxcall);
                    }
                    if (dxcall == rxdxcall)
                        waitHandle.Set();
                }
            }
        }

        public void send_watchlist(string watchlist, string MyCall, string MyLoc)
        {
            string qrg = qrg_from_settings();
            wtMessage Msg = new wtMessage(WTMESSAGES.ASWATCHLIST, Settings.Default.AS_My_Name, Settings.Default.AS_Server_Name, string.Concat(new string[]
            {
                qrg, ",", WCCheck.WCCheck.Cut(MyCall), ",", MyLoc, watchlist
            }));
            try
            {
                UdpClient client = new UdpClient(localep);
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

        public static IPAddress GetIpIFDefaultGateway()
        {
            return NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Where(n => n.GetIPProperties().GatewayAddresses.Count > 0 ) // only interfaces with a gateway
                .SelectMany(n => n.GetIPProperties()?.UnicastAddresses)
                .Where(g => g.Address.AddressFamily == AddressFamily.InterNetwork) // filter IPv4
                .Select(g => g?.Address)
                .FirstOrDefault(a => a != null);
        }

        /* called from BackgroundWorker bw_GetPlanes - reports results through bw_GetPlanes.ReportProgress */
        public bool GetPlanes(string mycall, string myloc, string dxcall, string dxloc)
        {

            string qrg = qrg_from_settings();

            this.mycall = mycall;
            this.dxcall = dxcall;
            wtMessage Msg = new wtMessage(WTMESSAGES.ASSETPATH, Settings.Default.AS_My_Name, Settings.Default.AS_Server_Name, 
                string.Concat(new string[] { qrg, ",", mycall, ",", myloc, ",", dxcall, ",", dxloc }));
            try
            {
                // https://stackoverflow.com/a/3297590
                // https://stackoverflow.com/questions/13634868/get-the-default-gateway/13635038#13635038
                UdpClient client = new UdpClient(localep);
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
                waitHandle.Reset();
                if (!waitHandle.WaitOne(rcvtimeout * 1000))
                {
                    bw_GetPlanes.ReportProgress(0, dxcall);
                    return false;
                }
                return true;
            }
            catch (Exception e1_596)
            {
                bw_GetPlanes.ReportProgress(0, null);
                bw_GetPlanes.ReportProgress(-1, dxcall);
                return false;
            }
        }

        private bool process_msg_asnearest(wtMessage Msg)
        {
            if (Msg.Msg == WTMESSAGES.ASNEAREST)
            {
                try
                {
                    PlaneInfoList infolist = new PlaneInfoList();
                    string[] a = Msg.Data.Split(new char[] { ',' });
                    DateTime utc = Convert.ToDateTime(a[0]).ToUniversalTime();
                    string mycall = a[1];
                    string myloc = a[2];
                    string dxcall = a[3];
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

        public string GetNearestPlanes(string call)
        {
            PlaneInfoList infolist = null;
            string result;
            if (planes.TryGetValue(call, out infolist))
            {
                string s = DateTime.UtcNow.ToString("HH:mm") + " [" + (DateTime.UtcNow - infolist.UTC).Minutes.ToString() + "mins ago]\n\n";
                int planes_per_Potential = 0;
                int current_Potential = 0;

                foreach (PlaneInfo info in infolist)
                {
                    if (current_Potential != info.Potential)
                    {
                        current_Potential = info.Potential;
                        planes_per_Potential = 0;
                    }
                    if (++planes_per_Potential > 5)
                        continue;
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
