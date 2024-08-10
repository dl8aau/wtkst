using Newtonsoft.Json;
using SIPSorcery.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebRTC
{
    /// <summary>
    /// Background worker for communication with AirScout's web based solution
    /// </summary>
    public class WebRTCWorker : BackgroundWorker
    {
        // GUID of the worker to identify the wtKST instance at AirScout
        string GUID = Guid.NewGuid().ToString().ToLower();

        // Peer connection
        RTCPeerConnection PC = null;

        // worker status
        WebRTCStatus Status = WebRTCStatus.UNDEFINED;

        /// <summary>
        /// Modified constructor
        /// </summary>
        public WebRTCWorker() 
        {
            this.WorkerReportsProgress = true;
            this.WorkerSupportsCancellation = true;
        }

        public const int ProgressError = -1;
        public const int ProgressInfo = 0;
        public const int ProgressStatus = 1;
        public const int ProgressMsg = 100;

        /// <summary>
        /// Modified DoWork procedure
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnDoWork(DoWorkEventArgs e)
        {
            base.OnDoWork(e);

            // get start args
            WebRTCStartArgs args = (WebRTCStartArgs)e.Argument;

            // check args
            if (args == null)
            {
                this.ReportProgress(ProgressError, "Cannot start worker, arguments missing!");
                return;
            }
            if (String.IsNullOrEmpty(args.ChannelID))
            {
                this.ReportProgress(ProgressError, "Cannot start worker, ChannelName missing!");
                return;
            }
            if (String.IsNullOrEmpty(args.AirScoutRootURL))
            {
                this.ReportProgress(ProgressError, "Cannot start worker, AirScoutRootURL missing!");
                return;
            }
            if (String.IsNullOrEmpty(args.AirScoutLoginURL))
            {
                this.ReportProgress(ProgressError, "Cannot start worker, AirScoutLoginURL missing!");
                return;
            }
            if (String.IsNullOrEmpty(args.AirScoutUsername))
            {
                this.ReportProgress(ProgressError, "Cannot start worker, AirScoutUsername missing!");
                return;
            }
            if (String.IsNullOrEmpty(args.AirScoutPassword))
            {
                this.ReportProgress(ProgressError, "Cannot start worker, AirScoutPassword missing!");
                return;
            }

            CookieContainer cookies = new CookieContainer();
            WebRTCWebStat webstat = null;

            DateTime start = DateTime.MinValue;

            this.ReportProgress(ProgressInfo, "Started.");

            while (!this.CancellationPending)
            {

                // try to get offers/answers status first
                // find out if already logged in / if PHP session cookie is still valid
                if ((Status == WebRTCStatus.ERROR) || (Status < WebRTCStatus.CONNECTED))
                {
                    try
                    {
                        using (WebClientEx client = new WebClientEx(cookies))
                        {
                            // get all this status entries from AirScout's web database
                            string url = args.AirScoutRootURL + "/wtksts.json?callerid=" + GUID;
                            client.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string response = client.DownloadString(url);
                            var wtksts = JsonConvert.DeserializeObject<WebRTCWebStat[]>(response);

                            // find own entry, if any and set webstat
                            webstat = wtksts.FirstOrDefault(wtkst => wtkst.callerid == GUID);
                        }

                        // clear answer, if any
                        if ((webstat != null) && (webstat.answer != "{}"))
                        {
                            using (WebClientEx client = new WebClientEx(cookies))
                            {
                                string url = args.AirScoutRootURL + "/wtkst/" + GUID + "/" + args.ChannelID + "/offer";
                                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                                string response = Encoding.UTF8.GetString(client.UploadData(url, "PUT", Encoding.UTF8.GetBytes("{}")));
                            }

                            if (webstat.answer.Contains("answer"))
                            {
                                this.ReportProgress(ProgressInfo, "Answer received: " + webstat.answer);
                            }
                            else
                            {
                                this.ReportProgress(ProgressInfo, "Answer discarded: " + webstat.answer);
                                webstat.answer = "{}";
                            }

                        }


                    }
                    catch (WebException ex) when (ex.Response is HttpWebResponse response)
                    {
                        // Not logged in!
                        if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Forbidden)
                        {
                            Status = WebRTCStatus.IDLE;
                            this.ReportProgress(ProgressError, "Not logged in! Check Username/Password");
                        }
                        // other error
                        else
                        {
                            string resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                            start = DateTime.MinValue;
                            Status = WebRTCStatus.ERROR;
                            this.ReportProgress(ProgressError, resp);
                        }
                    }
                    catch (Exception ex)
                    {
                        start = DateTime.MinValue;
                        Status = WebRTCStatus.ERROR;
                        this.ReportProgress(ProgressError, ex.ToString());
                    }
                }

                // log in to AirScout's web service and get a PHP session cookie
                if ((Status != WebRTCStatus.ERROR) && (Status == WebRTCStatus.IDLE))
                {
                    try
                    {
                        this.ReportProgress(ProgressInfo, "Logging in...");

                        Status = WebRTCStatus.LOGGINGIN;
                        using (WebClientEx client = new WebClientEx(cookies))
                        {
                            string url = args.AirScoutLoginURL;
                            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            string response = client.UploadString(url, "username=" + args.AirScoutUsername + "&password=" + args.AirScoutPassword);
                            Status = WebRTCStatus.LOGGEDIN;
                        }
                    }
                    catch (WebException ex) when (ex.Response is HttpWebResponse response)
                    {
                        // error while logging in --> wrong credentials?
                        if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Forbidden)
                        {
                            Status = WebRTCStatus.ERROR;
                            this.ReportProgress(ProgressError, "Error while logging in! Check Username/Password");

                            // wait here a bit longer 
                            Thread.Sleep(10000);
                        }
                        // other error
                        else
                        {
                            string resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                            start = DateTime.MinValue;
                            Status = WebRTCStatus.ERROR;
                            this.ReportProgress(ProgressError, resp);
                        }
                    }
                    catch (Exception ex)
                    {
                        start = DateTime.MinValue;
                        Status = WebRTCStatus.ERROR;
                        this.ReportProgress(ProgressError, ex.ToString());
                    }
                }

                // place an offer, if not already offering or connected
                if ((Status != WebRTCStatus.ERROR) && (Status < WebRTCStatus.OFFERING))
                {
                    try
                    {
                        Status = WebRTCStatus.OFFERING;

                        PC = await CreatePeerConnection();
                        RTCSessionDescriptionInit offer = PC.createOffer();
                        await PC.setLocalDescription(offer);

                        // set start for timeout
                        start = DateTime.UtcNow;

                        string json = JsonConvert.SerializeObject(offer, new Newtonsoft.Json.Converters.StringEnumConverter());

                        using (WebClientEx client = new WebClientEx(cookies))
                        {
                            string url = args.AirScoutRootURL + "/wtkst/" + GUID + "/" + args.ChannelID + "/offer";
                            client.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string response = Encoding.UTF8.GetString(client.UploadData(url, "PUT", Encoding.UTF8.GetBytes(json)));
                        }

                        this.ReportProgress(ProgressInfo, "Offer sent: " + json);

                    }
                    catch (WebException ex) when (ex.Response is HttpWebResponse response)
                    {
                        // Not logged in!
                        if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Forbidden)
                        {
                            Status = WebRTCStatus.IDLE;
                            this.ReportProgress(ProgressError, "Not logged in!");
                        }
                        // other error
                        else
                        {
                            string resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                            start = DateTime.MinValue;
                            Status = WebRTCStatus.ERROR;
                            this.ReportProgress(ProgressError, resp);
                        }
                    }
                    catch (Exception ex)
                    {
                        start = DateTime.MinValue;
                        Status = WebRTCStatus.ERROR;
                        this.ReportProgress(ProgressError, ex.ToString());
                    }
                }

                // wait until offer is answered
                if ((webstat != null) && !String.IsNullOrEmpty(webstat.answer) && (webstat.answer != "{}"))
                {
                    try
                    {
                        if (webstat.answer.Contains("answer"))
                        {
                            if ((Status != WebRTCStatus.ERROR) && (Status == WebRTCStatus.OFFERING) && (PC != null))
                            {
//                                var answer = JsonConvert.DeserializeObject<string>(webstat.answer);
                                RTCSessionDescriptionInit descriptioninit;
                                if (!RTCSessionDescriptionInit.TryParse(webstat.answer, out descriptioninit))
                                {
                                    start = DateTime.MinValue;
                                    Status = WebRTCStatus.ERROR;
                                }
                                else
                                {
                                    var result = PC.setRemoteDescription(descriptioninit);
                                    Status = WebRTCStatus.ANSWERED;
                                }
                            }
                        }
                    }
                    catch (WebException ex) when (ex.Response is HttpWebResponse response)
                    {
                        // Not logged in!
                        if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Forbidden)
                        {
                            Status = WebRTCStatus.IDLE;
                            this.ReportProgress(ProgressError, "Not logged in!");
                        }
                        // other error
                        else
                        {
                            string resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                            start = DateTime.MinValue;
                            Status = WebRTCStatus.ERROR;
                            this.ReportProgress(ProgressError, resp);
                        }
                    }
                    catch (Exception ex)
                    {
                        start = DateTime.MinValue;
                        Status = WebRTCStatus.ERROR;
                        this.ReportProgress(ProgressError, ex.ToString());
                    }
                }

                // open connection if answered
                if ((Status != WebRTCStatus.ERROR) && (Status == WebRTCStatus.ANSWERED) && (PC != null))
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(webstat.answer) && (webstat.answer != "{}"))
                        {
                            RTCSessionDescriptionInit descriptioninit;
                            if (!RTCSessionDescriptionInit.TryParse(webstat.answer, out descriptioninit))
                            {
                                start = DateTime.MinValue;
                                Status = WebRTCStatus.ERROR;
                            }
                            else
                            {
                                Status = WebRTCStatus.CONNECTING;
                            }

                        }
                    }
                    catch (WebException ex) when (ex.Response is HttpWebResponse response)
                    {
                        // Not logged in!
                        if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Forbidden)
                        {
                            Status = WebRTCStatus.IDLE;
                            this.ReportProgress(ProgressError, "Not logged in!");
                        }
                        // other error
                        else
                        {
                            string resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                            start = DateTime.MinValue;
                            Status = WebRTCStatus.ERROR;
                            this.ReportProgress(ProgressError, resp);
                        }
                    }
                    catch (Exception ex)
                    {
                        start = DateTime.MinValue;
                        Status = WebRTCStatus.ERROR;
                        this.ReportProgress(ProgressError, ex.ToString());
                    }
                }

                if ((Status != WebRTCStatus.ERROR) && (Status == WebRTCStatus.CONNECTING) && (PC != null))
                {
                    try
                    {
                        if (PC.connectionState == RTCPeerConnectionState.connected)
                        {
                            this.ReportProgress(ProgressInfo, "Connected!");

                            Status = WebRTCStatus.CONNECTED;
                        }
                    }
                    catch (WebException ex) when (ex.Response is HttpWebResponse response)
                    {
                        // Not logged in!
                        if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Forbidden)
                        {
                            Status = WebRTCStatus.IDLE;
                            this.ReportProgress(ProgressError, "Not logged in!");
                        }
                        // other error
                        else
                        {
                            string resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                            start = DateTime.MinValue;
                            Status = WebRTCStatus.ERROR;
                            this.ReportProgress(ProgressError, resp);
                        }
                    }
                    catch (Exception ex)
                    {
                        start = DateTime.MinValue;
                        Status = WebRTCStatus.ERROR;
                        this.ReportProgress(ProgressError, ex.ToString());
                    }
                }

                // check for timeout
                if (start != DateTime.MinValue)
                {
                    if ((DateTime.UtcNow - start).TotalSeconds > 10)
                    {
                        this.ReportProgress(ProgressError, "Timeout!");

                        start = DateTime.MinValue;
                        Status = WebRTCStatus.ERROR;
                    }
                }

                // clear offer if error occurred
                if (Status == WebRTCStatus.ERROR)
                {
                    using (WebClientEx client = new WebClientEx(cookies))
                    {
                        string url = args.AirScoutRootURL + "/wtkst/" + GUID;
                        client.Headers[HttpRequestHeader.ContentType] = "application/json";
                        try
                        {
                            string response = Encoding.UTF8.GetString(client.UploadData(url, "DELETE", new byte[0]));
                        }
                        catch
                        {
                            // do nothing here
                        }
                    }

                    Status = WebRTCStatus.IDLE;
                }

                // maintain state from peer connections
                if (PC != null)
                {
                    if ((PC.connectionState == RTCPeerConnectionState.closed) ||
                        (PC.connectionState == RTCPeerConnectionState.failed) ||
                        (PC.connectionState == RTCPeerConnectionState.disconnected))
                        Status = WebRTCStatus.IDLE;
                    else if (PC.connectionState == RTCPeerConnectionState.connected)
                    {
                        // stop timeout
                        start = DateTime.MinValue;
                    }
                }

                // display status
                this.ReportProgress(ProgressStatus, Status);

                Thread.Sleep(1000);
            }

            this.ReportProgress(ProgressInfo, "Stopped.");
            Status = WebRTCStatus.IDLE;
            this.ReportProgress(ProgressStatus, Status);
        }

        /// <summary>
        /// Creates a new RTC peer connection
        /// </summary>
        /// <returns></returns>
        private async Task<RTCPeerConnection> CreatePeerConnection()
        {

            RTCConfiguration config = new RTCConfiguration
            {
//                iceServers = new List<RTCIceServer> { new RTCIceServer { urls = "stun:stun.l.google.com:19302" } }
            };
            var pc = new RTCPeerConnection(config);

            var dc = await pc.createDataChannel("airscout", null);
            dc.onopen += () => Console.WriteLine($"Data channel {dc.label} opened.");
            dc.onclose += () => Console.WriteLine($"Data channel {dc.label} closed.");
            dc.onmessage += (datachan, type, data) =>
            {
                string msg;
                switch (type)
                {
                    case DataChannelPayloadProtocols.WebRTC_Binary_Empty:
                    case DataChannelPayloadProtocols.WebRTC_String_Empty:
                        Console.WriteLine($"Data channel {datachan.label} empty message type {type}.");
                        break;

                    case DataChannelPayloadProtocols.WebRTC_Binary:
                        msg = Encoding.UTF8.GetString(data);
                        Console.WriteLine($"Data channel {datachan.label} message {type} received: {msg}.");
                        this.ReportProgress(ProgressMsg, msg);
                        break;

                    case DataChannelPayloadProtocols.WebRTC_String:
                        msg = Encoding.UTF8.GetString(data);
                        Console.WriteLine($"Data channel {datachan.label} message {type} received: {msg}.");
                        this.ReportProgress(ProgressMsg, msg);

                        break;
                }
            };

            pc.onconnectionstatechange += (state) =>
            {
                Console.WriteLine($"Peer connection state change to {state}.");
                if (state == RTCPeerConnectionState.failed)
                {
                    pc.Close("ice disconnection");
                }
            };

            return pc;
        }

        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="msg">The message</param>
        /// <returns>TRUE if sent, FALSE if not</returns>
        public bool Send(string msg)
        {
            if (PC == null)
                return false;
            if (PC.connectionState != RTCPeerConnectionState.connected)
                return false;
            if (String.IsNullOrEmpty(msg))
                return false;

            try
            {
                RTCDataChannel channel = PC.DataChannels.ElementAt(0);
                channel.send(msg);

                this.ReportProgress(ProgressInfo, "Message sent: " + msg);

                return true;
            }
            catch (Exception ex)
            {
                this.ReportProgress(ProgressError, ex.ToString());
                return false;
            }
        }
    }
}
