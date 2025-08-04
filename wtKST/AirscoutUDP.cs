//#define DEBUG_AS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using WinTest;
using wtKST.Properties;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using static wtKST.AirScoutInterface;

namespace wtKST
{
    /// <summary>
    /// UDP based (old) protocol to communicate with the Airscout in the local network
    /// It is very similar to the Win-Test UDP protocol, just using port 9872 instead
    /// </summary>
    internal class AirscoutUDP
    {
        // access to planes needs to be protected by a lock!
        private Dictionary<string, PlaneInfoList> planes;

        private BackgroundWorker bw_GetPlanes;

        private Action<AS_STATE> asStateSetter;

        public bool Connected = false;

        private IPEndPoint localep;
        private WinTest.wtListener wtl;

        private EventWaitHandle msgWaitHandle;

        private string mycall;
        private string dxcall;
#if DEBUG_AS
        private Stopwatch stopWatch;
#endif
        public AirscoutUDP(ref BackgroundWorker bw_GetPlanes, ref Dictionary<string, PlaneInfoList> planes, Action<AS_STATE> asStateSetter)
        {
            this.bw_GetPlanes = bw_GetPlanes;
            this.planes = planes;
            this.asStateSetter = asStateSetter;

            msgWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

            localep = new IPEndPoint(GetIpIFDefaultGateway(), 0);

            wtl = new WinTest.wtListener(Settings.Default.AS_Port);
#if DEBUG_AS
            stopWatch = new Stopwatch();
#endif
        }

        public void Connect()
        {
            wtl.wtMessageReceived += wtMessageReceivedHandler;
        }

        public void Stop()
        {
            wtl.wtMessageReceived -= wtMessageReceivedHandler;
        }

        DateTime mTimestamp = DateTime.UtcNow;

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
                        bw_GetPlanes.ReportProgress(AirScoutInterface.ReportNewPlane, rxdxcall);
                    }
                    else
                    {
                        // report no planes available
                        bw_GetPlanes.ReportProgress(AirScoutInterface.ReportNoPlane, rxdxcall);
                    }
                    if (dxcall == rxdxcall)
                    {
#if DEBUG_AS
                        stopWatch.Stop();
                        Console.WriteLine("got AS " + dxcall + " " + e.Msg.Data + " in " + stopWatch.ElapsedMilliseconds);
#endif
                        msgWaitHandle.Set();

                        if (DateTime.UtcNow.Subtract(mTimestamp).TotalMilliseconds < 30000)
                            asStateSetter( AS_STATE.AS_IN_SYNC );
                        else
                            asStateSetter( AS_STATE.AS_UPDATING );
                        mTimestamp = DateTime.UtcNow;
                    }
#if DEBUG_AS
                    else
                        Console.WriteLine("dxcall " + this.dxcall + "!=" + rxdxcall);
#endif
                }
            }
        }

        public void SendWatchlist(string watchlist, string MyCall, string MyLoc)
        {
            string qrg = qrg_from_settings();
            wtMessage Msg = new wtMessage(WTMESSAGES.ASWATCHLIST, Settings.Default.AS_My_Name,
                                Settings.Default.AS_Local_Active ? Settings.Default.AS_Local_Name : Settings.Default.AS_Server_Name,
                                string.Concat(new string[]
            {
                qrg, ",", WCCheck.WCCheck.SanitizeCall(MyCall), ",", MyLoc, watchlist
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

        private string qrg_from_settings()
        {
            string qrg = "1440000";
            if (Settings.Default.AS_QRG == "50M")
            {
                qrg = "500000";
            }
            if (Settings.Default.AS_QRG == "70M")
            {
                qrg = "700000";
            }
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

        private IPAddress GetIpIFDefaultGateway()
        {
            return NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Where(n => n.GetIPProperties().GatewayAddresses.Count > 0) // only interfaces with a gateway
                .SelectMany(n => n.GetIPProperties()?.UnicastAddresses)
                .Where(g => g.Address.AddressFamily == AddressFamily.InterNetwork) // filter IPv4
                .Select(g => g?.Address)
                .FirstOrDefault(a => a != null);
        }

        public bool GetPlanes(string mycall, string myloc, string dxcall, string dxloc)
        {

            string qrg = qrg_from_settings();

            this.mycall = mycall;
            this.dxcall = dxcall;
#if DEBUG_AS
            Console.WriteLine("GetPlanes " + dxcall);
            stopWatch.Reset();
            stopWatch.Start();
#endif
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
                msgWaitHandle.Reset();
                if (!msgWaitHandle.WaitOne(rcvtimeout * 1000))
                {
#if DEBUG_AS
                    Console.WriteLine("timeout " + dxcall);
#endif
                    bw_GetPlanes.ReportProgress(0, dxcall);
                    asStateSetter( AS_STATE.AS_INACTIVE );
                    return false;
                }
                return true;
            }
            catch
            {
                bw_GetPlanes.ReportProgress(0, null);
                bw_GetPlanes.ReportProgress(-1, dxcall);
                asStateSetter( AS_STATE.AS_INACTIVE );
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
                        // ignore entries too far into the future
                        if (info.Mins < 30)
                            infolist.Add(info);
                    }
                    lock (planes)
                    {
                        planes.Remove(dxcall);
                        if (infolist.Count > 0)
                        {
                            infolist.Sort(new PlaneInfoComparer());
                            planes.Add(dxcall, infolist);
                            return true;
                        }
                    }
                }
                catch
                {
                }
            }
            return false;
        }

        public void ShowPath(string call, string loc, string MyCall, string MyLoc)
        {
            string qrg = qrg_from_settings();
            wtMessage Msg = new wtMessage(WTMESSAGES.ASSHOWPATH, Settings.Default.AS_My_Name,
                Settings.Default.AS_Local_Active ? Settings.Default.AS_Local_Name : Settings.Default.AS_Server_Name,
                string.Concat(new string[] { qrg, ",",  WCCheck.WCCheck.SanitizeCall(MyCall), ",",  MyLoc, ",",
                    WCCheck.WCCheck.SanitizeCall(call), ",", loc }));
            UdpClient client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            client.Client.ReceiveTimeout = 10000;
            IPEndPoint groupEp = new IPEndPoint(IPAddress.Broadcast, Settings.Default.AS_Port);
            client.Connect(groupEp);
            byte[] b = Msg.ToBytes();
            client.Send(b, b.Length);
            client.Close();
        }
    }
}
