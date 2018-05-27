using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private List<wtStat> _wtStatusList = new List<wtStat>();
        public IList<wtStat> wtStatusList { get { return _wtStatusList; } }

        public wtStatus()
        {
            wtl = new wtListener(WinTest.WinTestDefaultPort);
            wtl.wtMessageReceived += wtMessageReceivedHandler;
        }

        private void wtMessageReceivedHandler(object sender, wtListener.wtMessageEventArgs e)
        {
            //Console.WriteLine("WT Msg " + e.Msg.Msg + " src " +
            //e.Msg.Src + " dest " + e.Msg.Dst + " data " + e.Msg.Data + " checksum "
            //+ e.Msg.HasChecksum);
            if (e.Msg.Msg == WTMESSAGES.STATUS && e.Msg.HasChecksum)
            {
                string[] data = e.Msg.Data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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
                int index = _wtStatusList.FindIndex(x => x.from == e.Msg.Src );
                ulong freq, passfreq;
                if (!UInt64.TryParse(data[4], out freq))
                    freq = 0;
                if (!UInt64.TryParse(data[8], out passfreq))
                    passfreq = 0;
                wtStat w = new wtStat(e.Msg.Src, band, mode, freq*100UL, passfreq*100UL);
                if (index >= 0)
                {
                    _wtStatusList[index] = w;
                }
                else
                    _wtStatusList.Add(w);
            }
        }
    }
}
