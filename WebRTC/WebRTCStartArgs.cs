using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRTC
{
    public class WebRTCStartArgs
    {
        public string AirScoutLoginURL { get; set; } = "http://134.97.32.144/page/dist/login.php?login=1";
        public string AirScoutRootURL { get; set; } = "http://134.97.32.144/airscout/api/v1";
        public string AirScoutUsername { get; set; } = "";
        public string AirScoutPassword { get; set; } = "";
        public string ChannelID { get; set; } = "airscout";

    }
}
