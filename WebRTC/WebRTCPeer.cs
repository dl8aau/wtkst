using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using wtKST;
using wtKST.Properties;

namespace WebRTC
{
    public class WebRTCPeer
    {
        public Dictionary<string, PlaneInfoList> planes = new Dictionary<string, PlaneInfoList>();

        private BackgroundWorker bw_GetPlanes;
        private WebRTCWorker bw_WebRTC;

        private WebRTCStatus Status = WebRTCStatus.UNDEFINED;

        public bool Connected = false;

        public WebRTCPeer(ref BackgroundWorker bw_GetPlanes)
        {
            this.bw_GetPlanes = bw_GetPlanes;

            bw_WebRTC = new WebRTCWorker();
            bw_WebRTC.ProgressChanged += bw_WebRTC_ProgressChanged;

            // start WebRTC worker
            WebRTCStartArgs args = new WebRTCStartArgs();
            args.AirScoutUsername = Settings.Default.WS_Username;
            args.AirScoutPassword = Settings.Default.WS_Password;
            args.ChannelID = Settings.Default.WS_ChannelID;
            bw_WebRTC.RunWorkerAsync(args);

        }

        private void bw_WebRTC_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // status change received
            if (e.ProgressPercentage == 1)
            {
                Status = (WebRTCStatus)e.UserState;
            }
            // message received
            else if (e.ProgressPercentage == 100)
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
                            }

                        });

                        bw_GetPlanes.ReportProgress(2);
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
                bw_GetPlanes.ReportProgress(0, null);
                return false;
            }
        }

        public string GetNearestPlanes(string call)
        {
            if (Status != WebRTCStatus.CONNECTED)
                return "";

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
            if (Status != WebRTCStatus.CONNECTED)
                return;

            int qrg = QRGFromSettings();
            JSONRequest_SetPath sp = new JSONRequest_SetPath(Settings.Default.AS_My_Name, Settings.Default.AS_Local_Name, QRGFromSettings(), mycall, myloc, dxcall, dxloc);
            string json = JsonConvert.SerializeObject(sp);

            bw_WebRTC.Send(json);
        }

    }
}
