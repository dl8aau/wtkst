using System;
using System.Runtime.InteropServices;

namespace WinTest.DXLog
{
    public class NetworkMsgSync
    {

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct Sync
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public SyncQSODesc[] QSODesc;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SyncQSODesc
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string IDQSO;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string SynchPhase;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
            public string OriginStnID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string OriginIDQSO;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
            public string Timestamp;
        }

        private const int StructSize = 500;

        public SyncQSODesc[] QSOs = new SyncQSODesc[5];

        private Sync _sync;

        public void CreateStruct()
        {
            _sync = new Sync
            {
                QSODesc = QSOs
            };
        }

        private void GetValuesFromStruct()
        {
            QSOs = _sync.QSODesc;
        }

        public byte[] ToByteArray()
        {
            CreateStruct();

            try
            {
                var buffer = new byte[StructSize];
                var h = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                Marshal.StructureToPtr(_sync, h.AddrOfPinnedObject(), false);
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
                _sync = (Sync)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Sync));
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
