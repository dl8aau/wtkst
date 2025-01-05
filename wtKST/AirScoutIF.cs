using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using WebRTC;
using wtKST.Properties;

namespace wtKST
{
    class AirScoutInterface
    {
        public Dictionary<string, PlaneInfoList> planes = new Dictionary<string, PlaneInfoList>();

        private EventWaitHandle getPlanesWaitHandle;

        private BackgroundWorker bw_GetPlanes;

        private WebRTC.WebRTCPeer mWebRTC; // TODO more clever...
        private AirscoutUDP mUDPAS; // TODO more clever...

        public AirScoutInterface()
        {
            getPlanesWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

            this.bw_GetPlanes = new System.ComponentModel.BackgroundWorker();

            string mycall = WCCheck.WCCheck.SanitizeCall(Settings.Default.KST_UserName);

            // 
            // bw_GetPlanes
            // 
            this.bw_GetPlanes.WorkerReportsProgress = true;
            this.bw_GetPlanes.WorkerSupportsCancellation = true;
            this.bw_GetPlanes.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_GetPlanes_DoWork);
            this.bw_GetPlanes.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bw_GetPlanes_ProgressChanged);
            this.bw_GetPlanes.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_GetPlanes_RunWorkerCompleted);

            this.mUDPAS = new AirscoutUDP(ref bw_GetPlanes, ref planes, AsStateSetter); // more clever...
            this.mWebRTC = new WebRTCPeer(ref bw_GetPlanes, ref planes, AsStateSetter); // TODO more clever...
        }

        public void Dispose()
        {
        }

        public bool UDPBasedAS_isActive() { return Settings.Default.AS_Active; }
        public bool WebRTCAS_isActive() { return mWebRTC.IsActive(); }

        public bool IsActive() { return UDPBasedAS_isActive() || WebRTCAS_isActive();  }

        public void Connect()
        {
            if (WebRTCAS_isActive())
            {
                mUDPAS.Stop();
                ClearPlanesSendEvent();
                mWebRTC.Connect();
            }
            else 
            if (UDPBasedAS_isActive())
            {
                mWebRTC.Stop();
                ClearPlanesSendEvent();
                mUDPAS.Connect();
            }
            else // no service enabled, stop everything
            {
                mUDPAS.Stop();
                mWebRTC.Stop();
                ClearPlanesSendEvent();
            }

            if (!bw_GetPlanes.IsBusy)
                bw_GetPlanes.RunWorkerAsync();
        }

        public void OnFormClosing()
        {
            this.bw_GetPlanes.CancelAsync();
        }

        public enum AS_STATE
        {
            AS_INACTIVE,
            AS_UPDATING,
            AS_IN_SYNC,
        };

        private AS_STATE asState = AS_STATE.AS_INACTIVE;

        public AS_STATE ASState
        {
            get { return asState; }
            protected set
            {
                asState = value;
                if (ASStateChanged != null)
                {
                    ASStateChanged(this, new ASStateEventArgs(asState));
                }
            }
        }

        public event EventHandler<ASStateEventArgs> ASStateChanged;
        public class ASStateEventArgs : EventArgs
        {
            /// <summary>
            /// called when ASState changes
            /// </summary>
            public ASStateEventArgs(AS_STATE ASState)
            {
                this.ASState = ASState;
            }
            public AS_STATE ASState { get; private set; }
        }

        private void AsStateSetter(AS_STATE ASState) { this.ASState = ASState; }

        /// <summary>
        /// called from BackgroundWorker bw_GetPlanes - reports results through bw_GetPlanes.ReportProgress
        /// </summary>
        /// <param name="mycall"></param>
        /// <param name="myloc"></param>
        /// <param name="asarray"></param>
        /// <returns></returns>
        private bool GetPlanes(string mycall, string myloc, AS_Calls[] asarray)
        {
            int errors = 0;
            if (UDPBasedAS_isActive())
            {
                foreach (AS_Calls a in asarray)
                {
                    try
                    {
                        string dxloc = a.Locator;
                        // FIXME: handle /p etc.
                        string dxcall = a.Call;

                        if (UDPBasedAS_isActive())
                        {
                            if (!mUDPAS.GetPlanes(mycall, myloc, dxcall, dxloc))
                            {
                                errors++;
                                if (errors > 10)
                                {
                                    return false;
                                }

                            }
                        }
                        else
                            break; // can happen if UDP service is stopped
                        Thread.Sleep(200);
                    }
                    catch
                    {
                    }
                }
            }
            if (WebRTCAS_isActive())
            {
                List<JSONLocation> aslist = new List<JSONLocation>();
                asarray.ToList().ForEach(item => aslist.Add(new JSONLocation(item.Call, item.Locator)));
                if (!mWebRTC.GetPlanes(mycall, myloc, aslist))
                {
                    errors++;
                    if (errors > 10)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public class AS_Calls
        {
            public string Call;
            public string Locator;

            public AS_Calls(string call, string loc) { Call = call; Locator = loc; }
        };

        private AS_Calls[] mlocalAs_List;

        private void bw_GetPlanes_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!bw_GetPlanes.CancellationPending)
            {
                // we have a local copy of the current AS_list?
                if (mlocalAs_List == null || mlocalAs_List.Length == 0)
                {
                    getPlanesWaitHandle.Reset();
                    if (!getPlanesWaitHandle.WaitOne(1000))
                        continue; // break? Should not happen...
                }

                AS_Calls[] workingAs_List = new AS_Calls[mlocalAs_List.Length];
                // we need a working copy, as we need some time to process it
                lock (this)
                {
                    mlocalAs_List.CopyTo(workingAs_List, 0);
                }

                if (!GetPlanes(WCCheck.WCCheck.SanitizeCall(Settings.Default.KST_UserName), Settings.Default.KST_Loc, workingAs_List))
                {
                    Console.WriteLine("GetPlanes - error");
                    bw_GetPlanes.ReportProgress(ReportNoPlane, null);
                }
                Thread.Sleep(20000); // update every 20s, that should be enough
            }
            Console.WriteLine("bw_GetPlanes_DoWork done");
        }

        public event EventHandler<UpdateASStatusEventArgs> UpdateASStatusEvent;
        public class UpdateASStatusEventArgs : EventArgs
        {
            /// <summary>
            /// called when KSTState changes
            /// </summary>
            public UpdateASStatusEventArgs(string dxcall, string newtext)
            {
                this.dxcall = dxcall;
                this.newtext = newtext;
            }
            public string dxcall { get; private set; }
            public string newtext { get; private set; }
        }


        private void ClearPlanesSendEvent()
        {
            try
            {
                planes.Clear();

                if (UpdateASStatusEvent != null)
                {
                    UpdateASStatusEvent(this, new UpdateASStatusEventArgs(null, null));
                }
            }
            catch
            {
            }
        }

        public const int ReportNewPlane = 1;
        public const int ReportNoPlane = 0;
        public const int ReportError = -1;

        private void bw_GetPlanes_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string dxcall = (string)e.UserState;
            if (dxcall == null)
            {
                if (e.ProgressPercentage == ReportNoPlane)
                {
                    ClearPlanesSendEvent();
                }
                return;
            }

            string newtext = "";
            if (e.ProgressPercentage == ReportNewPlane)
            {
                newtext = GetNearestPlanePotential(dxcall);
            }
            else
            if (e.ProgressPercentage == ReportNoPlane)
            {
                //Console.WriteLine("remove " + dxcall);
                planes.Remove(dxcall);
            }
            else
            if (e.ProgressPercentage == ReportError)
            {
                Console.WriteLine("BW Getplanes err " + (string)e.UserState);
            }

            if (UpdateASStatusEvent != null)
            {
                UpdateASStatusEvent(this, new UpdateASStatusEventArgs(dxcall, newtext));
            }
        }

        private void bw_GetPlanes_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        /// <summary>
        /// Provide a list of calls to query from Airscout
        /// 
        /// </summary>
        /// <param name="AS_list"></param>
        public void update_AS_list(List<AS_Calls> AS_list)
        {
            mlocalAs_List = new AS_Calls[AS_list.Count];
            // bw_GetPlanes_DoWork and GetPlanes access it concurrently
            lock (this)
            {
                AS_list.CopyTo(mlocalAs_List);
            }
            if (mlocalAs_List.Length > 0)
                getPlanesWaitHandle.Set(); // unblock the getPlanes worker
        }

        public void ShowPath(string call, string loc, string MyCall, string MyLoc)
        {
            if (UDPBasedAS_isActive())
                mUDPAS.ShowPath(call, loc, MyCall, MyLoc);
            else
            if (WebRTCAS_isActive())
                mWebRTC.ShowPath(call, loc, MyCall, MyLoc);
        }

        public void SendWatchlist(string watchlist, string MyCall, string MyLoc)
        {
            if (UDPBasedAS_isActive())
                mUDPAS.SendWatchlist(watchlist, MyCall, MyLoc);
            else
            if (WebRTCAS_isActive())
                mWebRTC.SendWatchlist(watchlist, MyCall, MyLoc);
        }

        public string GetNearestPlanes(string call)
        {
            // really needed?
            if (WebRTCAS_isActive() && !mWebRTC.isConnected())
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
                        info.Mins>0 ? info.Mins : 0,
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

    }
}
