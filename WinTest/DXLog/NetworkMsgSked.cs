using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WinTest.DXLog
{
    public class NetworkMsgSked
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct Sked
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Callsign;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public byte[] SkedTime;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public byte[] Frequency;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] Mode;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 75)]
            public byte[] Comments;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] IsDeleted;
        }

        public int StructSize = 500;

        public string Callsign;
        public DateTime SkedTime;
        public string Frequency;
        public string Mode;
        public string Comments;
        public string IsDeleted;

        private Sked _sked;

        private string GetUnixTime(DateTime inTime)
        {
            var span = (inTime - new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return ((long)span.TotalSeconds).ToString();
        }

        private DateTime GetDtFromUnixTime(double unixTime)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dt.AddSeconds(unixTime);
        }

        private void CreateStruct()
        {
            _sked = new Sked
            {
                Callsign = GetBytes(Callsign ?? "", 16),
                SkedTime = GetBytes(GetUnixTime(SkedTime), 15),
                Frequency = GetBytes(Frequency ?? "", 15),
                Mode = GetBytes(Mode ?? "", 10),
                Comments = GetBytes(Comments ?? "", 75),
                IsDeleted = GetBytes(IsDeleted ?? "N", 2)
            };
        }

        private void GetValuesFromStruct()
        {
            Callsign = GetString(_sked.Callsign);
            SkedTime = GetDtFromUnixTime(double.Parse(GetString(_sked.SkedTime)));
            Frequency = GetString(_sked.Frequency);
            Mode = GetString(_sked.Mode);
            Comments = GetString(_sked.Comments);
            IsDeleted = GetString(_sked.IsDeleted);
        }

        public byte[] ToByteArray()
        {
            CreateStruct();

            try
            {
                var buffer = new byte[StructSize];
                var h = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                Marshal.StructureToPtr(_sked, h.AddrOfPinnedObject(), false);
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
                _sked = (Sked)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Sked));
                handle.Free();
                GetValuesFromStruct();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private byte[] GetBytes(string str, int size)
        {
            var bytes = new byte[size];
            Encoding.UTF8.GetBytes(str, 0, str.Length > size ? size : str.Length, bytes, 0);
            return bytes;
        }

        private string GetString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes).Trim('\0');
        }
    }
}
