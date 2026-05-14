using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace WinTest.DXLog
{
    public class NetworkMsgStatusInfo
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct StructStatusInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string StationID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public string Band;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public string Mode;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
            public string StationType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string QSYFreq;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string Radio1Freq;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string Radio2Freq;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string LoggedOp;

        }

        public int StructSize = 500;

        public string StationID;
        public string Band;
        public string Mode;
        public string StationType;
        public double QSYFreq;
        public double Radio1Freq;
        public double Radio2Freq;
        public string LoggedOp;
        public DateTime LastSeenUtc;

        public StructStatusInfo _statusInfo;

        public void CreateStruct()
        {
            _statusInfo = new StructStatusInfo
            {
                StationID = StationID,
                Band = Band,
                Mode = Mode,
                StationType = StationType,
                QSYFreq = QSYFreq.ToString(CultureInfo.InvariantCulture),
                Radio1Freq = Radio1Freq.ToString(CultureInfo.InvariantCulture),
                Radio2Freq = Radio2Freq.ToString(CultureInfo.InvariantCulture),
                LoggedOp = LoggedOp
            };
        }

        private void GetValuesFromStruct()
        {
            StationID = _statusInfo.StationID;
            Band = _statusInfo.Band;
            Mode = _statusInfo.Mode;
            StationType = _statusInfo.StationType;
            QSYFreq = Convert.ToDouble(_statusInfo.QSYFreq);
            Radio1Freq = Convert.ToDouble(_statusInfo.Radio1Freq);
            Radio2Freq = Convert.ToDouble(_statusInfo.Radio2Freq);
            LoggedOp = _statusInfo.LoggedOp;
            LastSeenUtc = DateTime.UtcNow;
        }

        public byte[] StructToByteArray(object _oStruct)
        {
            try
            {
                var buffer = new byte[StructSize];
                var h = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                Marshal.StructureToPtr(_oStruct, h.AddrOfPinnedObject(), false);
                h.Free();
                return buffer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetHeaderFromStructure(byte[] buffer)
        {
            try
            {
                var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                _statusInfo = (StructStatusInfo)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(StructStatusInfo));
                handle.Free();
                GetValuesFromStruct();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
