using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using wtKST;
using wtKST.Properties;

namespace WebRTC
{
    public class WebRTCPeer
    {
        private Dictionary<string, PlaneInfoList> planes;
        private BackgroundWorker bw_GetPlanes;
        private WebRTCWorker bw_WebRTC;

        private WebRTCStatus Status = WebRTCStatus.UNDEFINED;

        public bool Connected = false;

        public WebRTCPeer(ref BackgroundWorker bw_GetPlanes, ref Dictionary<string, PlaneInfoList> planes)
        {
            this.bw_GetPlanes = bw_GetPlanes;
            this.planes = planes;

            bw_WebRTC = new WebRTCWorker();
            bw_WebRTC.ProgressChanged += bw_WebRTC_ProgressChanged;
            bw_WebRTC.RunWorkerCompleted += Bw_WebRTC_RunWorkerCompleted;


        }

        private void Bw_WebRTC_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("WebRTC completed");
            if (e.Cancelled)
                Console.WriteLine("worker was canceled");
            if (mRestartWorker)
            {
                Console.WriteLine("restarting now");
                StartWorker();
                mRestartWorker = false;
            }
        }

        private WebRTCStartArgs mWebRTCargs = new WebRTCStartArgs();
        private bool mRestartWorker = false;

        public void Connect()
        {
            Console.WriteLine("WebRTC connect");

            // if WebRTC worker is already running, check if parameters changed
            if (bw_WebRTC.IsBusy)
            {
                if (mWebRTCargs.AirScoutUsername != Settings.Default.WS_Username ||
                    mWebRTCargs.AirScoutPassword != Settings.Default.WS_Password ||
                    mWebRTCargs.AirScoutLoginURL != Settings.Default.WS_LoginURL ||
                    mWebRTCargs.AirScoutRootURL != Settings.Default.WS_API_URL ||
                    mWebRTCargs.ChannelID != Settings.Default.AS_Server_Name)
                {
                    Console.WriteLine("WebRTC restart");
                    bw_WebRTC.CancelAsync();
                    mRestartWorker = true;
                }
                else
                    mRestartWorker = false;
                return; // no restart needed or restart in the completion event
            }
            StartWorker();
        }

        private void StartWorker()
        {
            // start WebRTC worker
            mWebRTCargs.AirScoutUsername = Settings.Default.WS_Username;
            mWebRTCargs.AirScoutPassword = Settings.Default.WS_Password;
            mWebRTCargs.AirScoutLoginURL = Settings.Default.WS_LoginURL;
            mWebRTCargs.AirScoutRootURL = Settings.Default.WS_API_URL;
            mWebRTCargs.ChannelID = Settings.Default.AS_Server_Name;
            bw_WebRTC.RunWorkerAsync(mWebRTCargs);
        }
        public void Stop()
        {
            Console.WriteLine("WebRTC stop");
            if (bw_WebRTC.IsBusy)
                bw_WebRTC.CancelAsync();
        }

        public bool IsActive() { return Settings.Default.WS_Active; }

        private void bw_WebRTC_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // status change received
            if (e.ProgressPercentage == WebRTCWorker.ProgressStatus)
            {
                Status = (WebRTCStatus)e.UserState;
            }
            // report progress
            else if (e.ProgressPercentage == WebRTCWorker.ProgressInfo)
            {
                Console.WriteLine("BW_WebRTC progress: " + (String)e.UserState);
            }
            // report error
            else if (e.ProgressPercentage == WebRTCWorker.ProgressError)
            {
                Console.WriteLine("BW_WebRTC error: " + (String)e.UserState);
                // propagate to interface
                bw_GetPlanes.ReportProgress(AirScoutInterface.ReportError, e.UserState);
            }
            // message received
            else if (e.ProgressPercentage == WebRTCWorker.ProgressMsg)
            {
                string msg = (string)e.UserState;
                try
                {
                    // ignore all messages which are not JSON
                    if (!msg.StartsWith("{") || !msg.EndsWith("}"))
                       return;

                    JSONResponse response = JsonConvert.DeserializeObject<JSONResponse>(msg);
                    if (response.task == "get_nearestplanes")
                    {
                        JSONResponse_GetNearestPlanes nearestplanes = JsonConvert.DeserializeObject<JSONResponse_GetNearestPlanes>(msg);

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
                                bw_GetPlanes.ReportProgress(AirScoutInterface.ReportNewPlane, item.call);
                            }
                            else // report no planes available
                                bw_GetPlanes.ReportProgress(AirScoutInterface.ReportNoPlane, item.call);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[WebSocket] Error while receiving message:: " + ex.Message);
                }
            }

        /*
        if (e.ProgressPercentage == -1)
            SayError((string)e.UserState);
        if (e.ProgressPercentage == 0)
            SayInfo((string)e.UserState);
        else if (e.ProgressPercentage == 1)
        {
            WebRTCStatus status = (WebRTCStatus)e.UserState;
            SayStatus(status);
        }
        else if (e.ProgressPercentage == 100)
            SayInfo("Message received: " + (string)e.UserState);
        */
    }

        private int QRGFromSettings()
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

            if (Status != WebRTCStatus.CONNECTED)
                return;

            try
            {
                if (String.IsNullOrEmpty(watchlist) || (watchlist == ","))
                    return;

                JSONRequest_SetWatchlist wl = new JSONRequest_SetWatchlist(Settings.Default.AS_My_Name, "", QRGFromSettings(), mycall, myloc, watchlist);
                string json = JsonConvert.SerializeObject(wl);

                bw_WebRTC.Send(json);
            }
            catch
            {
            }
        }

        public bool GetPlanes(string mycall, string myloc, List<JSONLocation> aslist)
        {
            if (Status != WebRTCStatus.CONNECTED)
                return false;

            int qrg = QRGFromSettings();

            try
            {
                JSONRequest_GetNearestPlanes req = new JSONRequest_GetNearestPlanes(Settings.Default.AS_My_Name, Settings.Default.AS_Local_Name, qrg, mycall, myloc, aslist);
                string json = JsonConvert.SerializeObject(req);

                bw_WebRTC.Send(json);

                return true;
            }
            catch
            {
                bw_GetPlanes.ReportProgress(AirScoutInterface.ReportNoPlane, null);
                return false;
            }
        }

        public bool isConnected()
        {
            return (Status == WebRTCStatus.CONNECTED);
        }

        public void ShowPath(string dxcall, string dxloc, string mycall, string myloc)
        {
            if (Status != WebRTCStatus.CONNECTED)
                return;

            int qrg = QRGFromSettings();
            JSONRequest_SetPath sp = new JSONRequest_SetPath(Settings.Default.AS_My_Name, Settings.Default.AS_Local_Name, QRGFromSettings(), mycall, myloc, dxcall, dxloc);
            string json = JsonConvert.SerializeObject(sp);

            bw_WebRTC.Send(json);
        }

    }
}
