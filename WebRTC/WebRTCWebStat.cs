using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRTC
{
    public class WebRTCWebStat
    {
        public string myid  = "";
        public string callerid { get; set; } = "";
        public string offer { get; set; } = "";
        public DateTime offerts { get; set; } = DateTime.MinValue;
        public string answer { get; set; } = "";
        public DateTime answerts { get; set; } = DateTime.MinValue;
        public int status { get; set; } = 0;
    }
}
