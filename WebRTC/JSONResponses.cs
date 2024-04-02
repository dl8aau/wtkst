using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebRTC
{
    [Serializable]
    public class JSONNearestPlanes
    {
        public string call { get; set; } = "";
        public string loc { get; set; } = "";
        public List<JSONNearestPlane> data { get; set; } = new List<JSONNearestPlane>();

    }

    [Serializable]
    public class JSONNearestPlane
    {
        public string call { get; set; } = "";
        public string cat { get; set; } = "";
        public int int_qrb { get; set; } = int.MaxValue;
        public int int_potential { get; set; } = 0;
        public int int_min { get; set; } = int.MaxValue;

    }

    [Serializable]
    public class JSONResponse
    {
        public string task { get; set; } = "get_nearest_planes";
    }
    
    [Serializable]
    public class JSONResponse_GetNearestPlanes : JSONResponse
    {
        public List<JSONNearestPlanes> data { get; set; } = new List<JSONNearestPlanes>();
    }
}
