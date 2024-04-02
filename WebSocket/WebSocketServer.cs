using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
//using WebSocketSharp;
using wtKST;
using wtKST.Properties;

namespace WebSocket
{
    /*
    public class AirScoutService : WebSocketSharp.Server.WebSocketBehavior
    {
        public event EventHandler<MessageEventArgs> onMessage;
        public event EventHandler<ErrorEventArgs> onError;
        public event EventHandler<CloseEventArgs> onClose;

        public Guid GUID = Guid.NewGuid();

        public bool Connected = false;
        protected override void OnMessage(MessageEventArgs e)
        {
            if (onMessage != null)
            {
                onMessage(this, e);
            }
        }

        protected override void OnError(ErrorEventArgs e)
        {
            if (onError != null)
            {
                onError(this, e);
            }

            if (this.ReadyState == WebSocketState.Open)
            {
                this.Close();
                this.Sessions.CloseSession(this.ID);
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Connected = false;
            
            if (onClose != null)
            {
                onClose(this, e);
            }
        }

        protected override void OnOpen()
        {
            Connected = true;
            base.OnOpen();
        }

        public void SendMessage(string msg, AirScoutService client = null)
        {
            if (client != null)
                this.Send(msg);
            else
                this.Sessions.Broadcast(msg);

        }
    }
    public class WebSocketServer
    {
        public Dictionary<string, PlaneInfoList> planes = new Dictionary<string, PlaneInfoList>();

        private IPEndPoint localep;
        private BackgroundWorker bw_GetPlanes;
        private WebSocketSharp.Server.WebSocketServer WSS;

        private EventWaitHandle waitHandle;

        public List<AirScoutService> AirScoutServices = new List<AirScoutService>();

        public bool Connected = false;

        public WebSocketServer(ref BackgroundWorker bw_GetPlanes)
        {

            this.localep = new IPEndPoint(GetIpIFDefaultGateway(), Settings.Default.WS_Port);
            this.bw_GetPlanes = bw_GetPlanes;
            waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

            Action<AirScoutService> OnOpen = OnAirScoutOpen;
            this.WSS = new WebSocketSharp.Server.WebSocketServer("ws://" + localep.ToString());
            this.WSS.AddWebSocketService<AirScoutService>("/AirScout", OnAirScoutOpen);
            this.WSS.Start();
        }

        public void OnAirScoutOpen(AirScoutService client)
        {
            client.onMessage += OnAirScoutMessage;
            client.onError += OnAirScoutError;
            client.onClose += OnAirScoutClose;

            if (!AirScoutServices.Contains(client))
            {
                AirScoutServices.Add(client);
            }

            Connected = client.Connected;
            Console.WriteLine("[WebSocket] Client connected: " + client.ID + " (" + AirScoutServices.Count.ToString() + ")");
        }

        public void OnAirScoutMessage(Object sender, MessageEventArgs e)
        {
            AirScoutService client = (sender as AirScoutService);
            if (!String.IsNullOrEmpty(e.Data))
            {
                if (e.Data.Length < 255)
                    Console.WriteLine("[WebSocket] Message received: " + e.Data);
                else
                    Console.WriteLine("[WebSocket] Message received, length: " + e.Data.Length + " bytes.");

                try
                {
                    // ignore all messages which are not JSON
                    if (!e.Data.StartsWith("{") || !e.Data.EndsWith("}"))
                        return;

                    JSONResponse response = JsonConvert.DeserializeObject<JSONResponse>(e.Data);
                    if (response.task == "get_nearestplanes")
                    {
                        JSONResponse_GetNearestPlanes nearestplanes = JsonConvert.DeserializeObject<JSONResponse_GetNearestPlanes>(e.Data);

                        planes.Clear();

                        nearestplanes.data.ForEach(item =>
                        {
                            PlaneInfoList infolist = new PlaneInfoList();

                            item.data.ForEach(plane =>
                            {
                                if (plane.int_potential > 0)
                                {
                                    PlaneInfo info = new PlaneInfo(plane.call, plane.cat, plane.int_qrb, plane.int_potential, plane.int_min);
                                    infolist.Add(info);
                                }
                            });

                            if (infolist.Count > 0)
                            {
                                infolist.Sort(new PlaneInfoComparer());

                                planes.Add(item.call, infolist);
                            }



                        });

                        bw_GetPlanes.ReportProgress(2);

                        waitHandle.Set();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[WebSocket] Error while receiving message:: " + ex.Message);
                }
            }

        }

        public void OnAirScoutError(Object sender, ErrorEventArgs e)
        {
            AirScoutService client = (sender as AirScoutService);
            Connected = client.Connected;
            Console.WriteLine("[WebSocket] Error, reason: " + e.Message);
        }

        public void OnAirScoutClose(Object sender, CloseEventArgs e)
        {
            AirScoutService client = (sender as AirScoutService);

            if (AirScoutServices.Contains(client))
            {
                AirScoutServices.Remove(client);
            }

            Connected = client.Connected;
            Console.WriteLine("[WebSocket] Closed, reason: " + e.Reason);
        }

        public static IPAddress GetIpIFDefaultGateway()
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

        public int QRGFromSettings()
        {
            int qrg = 144;
            if (Settings.Default.AS_QRG == "432M")
            {
                qrg = 432;
            }
            else if (Settings.Default.AS_QRG == "1.2G")
            {
                qrg = 1296;
            }
            else if (Settings.Default.AS_QRG == "2.3G")
            {
                qrg = 2320;
            }
            else if (Settings.Default.AS_QRG == "3.4G")
            {
                qrg = 3400;
            }
            else if (Settings.Default.AS_QRG == "5.7G")
            {
                qrg = 5760;
            }
            else if (Settings.Default.AS_QRG == "10G")
            {
                qrg = 10368;
            }
            return qrg;
        }


        public void SendWatchlist(string watchlist, string mycall, string myloc)
        {
            if (!Settings.Default.WS_Active)
                return;

            try
            {
                if (String.IsNullOrEmpty(watchlist) || (watchlist == ","))
                    return;

                JSONRequest_SetWatchlist wl = new JSONRequest_SetWatchlist(Settings.Default.AS_My_Name, "", QRGFromSettings(), mycall, myloc, watchlist);
                string json = JsonConvert.SerializeObject(wl);

                WebSocketSharp.Server.WebSocketServiceHost host = null;
                if (WSS.WebSocketServices.TryGetServiceHost("/AirScout", out host))
                {
                    host.Sessions.Broadcast(json);
                }

            }
            catch
            {
            }
        }

        public bool GetPlanes(string mycall, string myloc, List<WebSocket.JSONLocation> aslist)
        {

            int qrg = QRGFromSettings();

            try
            {
                JSONRequest_GetNearestPlanes req = new JSONRequest_GetNearestPlanes(Settings.Default.AS_My_Name, Settings.Default.AS_Local_Name, qrg, mycall, myloc, aslist);
                string json = JsonConvert.SerializeObject(req);

                // use only the first service to get nearest planes
                if (AirScoutServices.Count > 0)
                {
                    AirScoutServices[0].SendMessage(json);
                }

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

                // create a timeout according to amount of planes to calculate
                if (!waitHandle.WaitOne(rcvtimeout * 1000 * aslist.Count))
                {
                    bw_GetPlanes.ReportProgress(0, null);
                    return false;
                }

                return true;
            }
            catch
            {
                bw_GetPlanes.ReportProgress(0, null);
                return false;
            }
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
                result = infolist[0].Potential + "," + infolist[0].Category + "," + infolist[0].Mins;
            }
            else
            {
                result = "0";
            }
            return result;
        }

        public void ShowPath(string dxcall, string dxloc, string mycall, string myloc)
        {
            int qrg = QRGFromSettings();
            JSONRequest_SetPath sp = new JSONRequest_SetPath(Settings.Default.AS_My_Name, Settings.Default.AS_Local_Name, QRGFromSettings(), mycall, myloc, dxcall, dxloc);
            string json = JsonConvert.SerializeObject(sp);

            WebSocketSharp.Server.WebSocketServiceHost host = null;
            if (WSS.WebSocketServices.TryGetServiceHost("/AirScout", out host))
            {
                host.Sessions.Broadcast(json);
            }
        }
    }
*/
}
