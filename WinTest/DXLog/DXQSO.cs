using System;
using System.Runtime.InteropServices;

namespace WinTest.DXLog
{
    public class DXQSO
    {
        // Properties
        public int IDQSO => _idqso;

        public string Band => _band;

        public int Period => _period;

        public decimal Frequency => _frequency;

        public string Mode => _mode;

        public DateTime QSOTime => _qsoTime;

        public string Callsign => _callsign;

        public string Sent => _sent;

        public int Nr => _nr;

        public string Rcvd => _rcvd;

        public string RecInfo => _recInfo;

        public string RecInfo2 => _recInfo2;

        public string RecInfo3 => _recInfo3;

        public string Stn => _stn;

        public string OriginStnID => _originStnId;

        public int OriginIDQSO => _originIdqso;

        public DateTime TimeStamp => _timeStamp;

        public string Operator => _operator;

        public bool XQSO => _xqso;

        public Guid QSOGuid => _qsoGuid;


        // The following code converts a DXQSO to and from a buffer which
        // can be sent across the network.

        // VERY IMPORTANT: There are applications that rely on the exact format of QSO network
        // messages. Most notable is Clublog's Livestream gateway. Extending the message will
        // probably not break any other applications but changing the location of e.g., Operator, will.

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct StructQSO
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] IDQSO;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] Band;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] Period;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public byte[] Frequency;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] Mode;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public byte[] QSOTime;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Callsign;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] Sent;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] Nr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Rcvd;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] RecInfo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] RecInfo2;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] RecInfo3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public byte[] Stn;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public byte[] OriginStnID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] OriginIDQSO;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public byte[] Timestamp;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] QSOGuid;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Operator;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] XQSO;
        }

        private int StructSize = 254;
        private int _idqso;
        private string _band;
        private int _period;
        private decimal _frequency;
        private string _mode;
        private DateTime _qsoTime;
        private string _callsign;
        private string _sent;
        private int _nr;
        private string _rcvd;
        private string _recInfo;
        private string _recInfo2;
        private string _recInfo3;
        private string _stn;
        private string _originStnId;
        private int _originIdqso;
        private DateTime _timeStamp;
        private string _operator;
        private bool _xqso;
        private Guid _qsoGuid;

        public void GetHeaderFromStructure(byte[] inBuffer)
        {
            byte[] buffer = new byte[StructSize];
            Array.Copy(inBuffer, 0, buffer, 0, StructSize);
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            StructQSO structQSO = (StructQSO)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(StructQSO));
            handle.Free();

            int.TryParse(GetString(structQSO.IDQSO), out _idqso);
            int.TryParse(GetString(structQSO.Period), out _period);
            _band = GetString(structQSO.Band);
            decimal.TryParse(GetString(structQSO.Frequency), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.GetCultureInfo("en-US"), out _frequency);
            _mode = GetString(structQSO.Mode);
            _qsoTime = GetDTFromUnixTime(double.Parse(GetString(structQSO.QSOTime)));
            _callsign = GetString(structQSO.Callsign);
            _sent = GetString(structQSO.Sent);
            int.TryParse(GetString(structQSO.Nr), out _nr);
            _rcvd = GetString(structQSO.Rcvd);
            _recInfo = GetString(structQSO.RecInfo);
            _recInfo2 = GetString(structQSO.RecInfo2);
            _recInfo3 = GetString(structQSO.RecInfo3);
            _stn = GetString(structQSO.Stn);
            int.TryParse(GetString(structQSO.OriginIDQSO), out _originIdqso);
            _originStnId = GetString(structQSO.OriginStnID);

            if (double.TryParse(GetString(structQSO.Timestamp), out double ts))
            {
                _timeStamp = GetDTFromUnixTime(ts);
            }

            try
            {
                _qsoGuid = new Guid(structQSO.QSOGuid);
            }
            catch { }

            _operator = GetString(structQSO.Operator);
            _xqso = GetString(structQSO.XQSO) == "Y";
        }

        private string GetString(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes).Trim('\0');
        }

        private DateTime GetDTFromUnixTime(double unixTime)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dt.AddSeconds(unixTime);
        }
    }
}
