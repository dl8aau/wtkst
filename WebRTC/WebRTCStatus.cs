using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRTC
{
    public enum WebRTCStatus
    {
        UNDEFINED = 0,
        IDLE = 1,
        LOGGINGIN = 2,
        LOGGEDIN = 3,
        OFFERING = 4,
        OFFERED = 5,
        ANSWERED = 6,
        CONNECTING = 7,
        CONNECTED = 8,
        ERROR = 128
    }
}
