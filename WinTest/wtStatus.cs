using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Timers;

namespace WinTest
{
    public class wtStatus
    {
        private wtListener wtl;

        public class wtStat
        {
            public string from { get; set; }
            public string band;
            public string mode;
            public ulong freq; // current radio frequency in Hz
            public ulong passfreq; // current passfrequency in Hz
            public DateTime timestamp;

            public wtStat(string from, string band, string mode, ulong freq, ulong passfreq)
            {
                this.from = from; this.band = band; this.mode = mode;
                this.freq = freq; this.passfreq = passfreq;
                this.timestamp = DateTime.Now;
            }
        };

        //private BindingList<wtStat> _wtStatusList = new BindingList<wtStat>();
        //public BindingList<wtStat> wtStatusList { get { return _wtStatusList; } }
        public readonly BindingList<wtStat> wtStatusList;

        private readonly SynchronizationContext _context = SynchronizationContext.Current;
        private System.Timers.Timer ti_check_timestamps;
        private System.Timers.Timer ti_send_status_msg;

        private string my_wtname;

        public wtStatus(string wtname)
        {
            my_wtname = wtname;
            wtl = new wtListener(WinTest.WinTestDefaultPort);
            wtl.wtMessageReceived += wtMessageReceivedHandler;
            wtStatusList = new BindingList<wtStat>();

            ti_check_timestamps = new System.Timers.Timer();
            ti_check_timestamps.Enabled = true;
            ti_check_timestamps.Interval = 60000; // check every minute
            ti_check_timestamps.Elapsed += new System.Timers.ElapsedEventHandler(this.ti_check_timestamps_Tick);

            ti_send_status_msg = new System.Timers.Timer();
            ti_send_status_msg.Enabled = true;
            ti_send_status_msg.Interval = 30000; // every 5 minutes
            ti_send_status_msg.Elapsed += Ti_send_status_msg_Elapsed;

        }

        private void Ti_send_status_msg_Elapsed(object sender, ElapsedEventArgs e)
        {
            wtMessage Msg = new wtMessage(WTMESSAGES.GAB, my_wtname, "KEEPALIVE", "KEEPALIVE");
            WinTest.send(Msg);
        }

        private void wtMessageReceivedHandler(object sender, wtListener.wtMessageEventArgs e)
        {
            //Console.WriteLine("WT Msg " + e.Msg.Msg + " src " +
            //e.Msg.Src + " dest " + e.Msg.Dst + " data " + e.Msg.Data + " checksum "
            //+ e.Msg.HasChecksum);
            /* STATUS "mm" "" 0 12 0 0 0 "0" 0 "1" 1440400 ""
             * 
             *                  band
             *                    mode
             *                       radio1 is primary
             *                         radio1 freq
             *                           ?
             *                               radio2 freq
             *                                 ?
             *                                         pass_freq
             *                                             ?
             */
            if (e.Msg.Msg == WTMESSAGES.STATUS && e.Msg.HasChecksum)
            {
                string[] data = e.Msg.Data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (data.Length == 10 || data.Length == 9)
                {
                    string band;
                    try
                    {
                        band = Enum.Parse(typeof(WTBANDS), data[1]).ToString().Replace("Band", "");
                    }
                    catch
                    {
                        band = "";
                    }

                    string mode;
                    try
                    {
                        mode = Enum.Parse(typeof(WTMODE), data[2]).ToString().Replace("Mode", "");
                    }
                    catch
                    {
                        mode = "";
                    }

                    //Console.WriteLine("STATUS: from " + e.Msg.Src + " band " + band + " mode " + mode +
                    //    " freq " + data[4] + " pass " + data[8]);

                    ulong freq, passfreq;
                    if (!UInt64.TryParse(data[4], out freq))
                        freq = 0;
                    if (!UInt64.TryParse(data[8], out passfreq))
                        passfreq = 0;
                    wtStat w = new wtStat(e.Msg.Src, band, mode, freq * 100UL, passfreq * 100UL);
                    // we need to make sure, wtStatusList is updated in the UI thread, otherwise the ListChanged event
                    // does not reach the UI elements
                    var th = new Thread(() =>
                    {
                        _context.Send(o => wtStatusListAdd_UIthread(e.Msg.Src, w), null);
                    })
                    { IsBackground = true };
                    th.Start();
                }
            }
        }

        private void wtStatusListAdd_UIthread(string src, wtStat w)
        {
            var el = wtStatusList.SingleOrDefault(x => x.from == src);

            if (el != null)
            {
                el = w;
            }
            else
            {
                wtStatusList.Add(w);
            }
        }
        private void ti_check_timestamps_Tick(object sender, ElapsedEventArgs e)
        {
            var th = new Thread(() =>
            {
                _context.Send(o => wtStatusListRemoveExpired_UIthread(), null);
            })
            { IsBackground = true };
            th.Start();
        }

        private void wtStatusListRemoveExpired_UIthread()
        {
            for (int i = 0; i < wtStatusList.Count; i++)
            {
                var el = wtStatusList[i];
                if (el != null)
                {
                    if (DateTime.Now.Subtract(el.timestamp).TotalMinutes > 3) // 3 min timeout
                    {
                        wtStatusList.Remove(el);
                    }
                }
            }
        }
    }
}
