using System;
using System.Runtime.InteropServices;

namespace WinTest.DXLog
{
    public class NetworkMsgSyncRequest
    {

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SyncRequest
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string IDQSO;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string SynchPhase;
        }

        public int StructSize = 500;

        public string IDQSO;
        public int SyncPhase;

        private SyncRequest _syncRequest;

        public void CreateStruct()
        {
            _syncRequest = new SyncRequest
            {
                IDQSO = IDQSO,
                SynchPhase = SyncPhase.ToString()
            };
        }

        private void GetValuesFromStruct()
        {
            IDQSO = _syncRequest.IDQSO;
            int.TryParse(_syncRequest.SynchPhase, out SyncPhase);
        }

        public byte[] ToByteArray()
        {
            CreateStruct();

            try
            {
                var buffer = new byte[StructSize];
                var h = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                Marshal.StructureToPtr(_syncRequest, h.AddrOfPinnedObject(), false);
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
                _syncRequest = (SyncRequest)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SyncRequest));
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
