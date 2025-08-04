using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebRTC
{
    [Serializable]
    public class JSONLocation
    {
        public string call { get; set; } = "";
        public string loc { get; set; } = "";

        public JSONLocation(string call, string loc)
        {
            this.call = call;
            this.loc = loc;
        }

    }

    [Serializable]
    public class JSONRequest
    {
        public string task { get; internal set; } = "";
        public string from { get; internal set; } = "";
        public string to { get; internal set; } = "";

    }

    [Serializable]
    public class JSONRequest_SetWatchlist : JSONRequest
    {
        public string mycall { get; set; } = "";
        public string myloc { get; set; } = "";

        public int qrg { get; set; } = 0;
        public List<JSONLocation> data { get; set; } = new List<JSONLocation>();
        public JSONRequest_SetWatchlist(string from, string to, int qrg, string mycall, string myloc, string watchlist)
        {
            this.task = "set_watchlist";
            this.from = from;
            this.to = to;

            this.qrg = qrg;
            this.mycall = mycall;
            this.myloc = myloc;

            // remove leading/trailing separators from watchlist
            watchlist = watchlist.Trim(',');
            string[] a = watchlist.Split(',');
            for (int i = 0; i < a.Length - 1; i+= 2)
            {
                JSONLocation l = new JSONLocation(a[i], a[i + 1]) ;
                data.Add(l);
            }
        }
    }

    [Serializable]
    public class JSONRequest_GetNearestPlanes : JSONRequest
    {
        public string mycall { get; set; } = "";
        public string myloc { get; set; } = "";

        public int qrg { get; set; } = 0;
        public List<JSONLocation> data { get; set; } = new List<JSONLocation>();
        public JSONRequest_GetNearestPlanes(string from, string to, int qrg, string mycall, string myloc, List<JSONLocation> aslist)
        {
            this.task = "get_nearestplanes";
            this.from = from;
            this.to = to;

            this.qrg = qrg;
            this.mycall = mycall;
            this.myloc = myloc;

            this.data = aslist;
        }
    }

    [Serializable]
    public class JSONRequest_SetPath : JSONRequest
    {
        public string mycall { get; set; } = "";
        public string myloc { get; set; } = "";
        public string dxcall { get; set; } = "";
        public string dxloc { get; set; } = "";

        public int qrg { get; set; } = 0;
        public JSONRequest_SetPath(string from, string to, int qrg, string mycall, string myloc, string dxcall, string dxloc)
        {
            this.task = "set_path";
            this.from = from;
            this.to = to;

            this.qrg = qrg;
            this.mycall = mycall;
            this.myloc = myloc;
            this.dxcall = dxcall;
            this.dxloc = dxloc;

        }
    }


}
