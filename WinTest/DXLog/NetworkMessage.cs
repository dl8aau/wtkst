using System.Runtime.InteropServices;

namespace WinTest.DXLog
{
    public class NetworkMessage
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct StructNetworkMsg
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public char[] MsgSender;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public char[] MsgDestination;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public char[] MsgType;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public char[] MsgID;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 500)]
            public byte[] MsgData;
        }

        public int StructSize = 580;
        public string MsgSender;
        public string MsgDestination;
        public string MsgType;
        public string MsgID;
        public byte[] MsgData;

        private StructNetworkMsg _networkMessage;
        
        private void CreateStruct()
        {
            _networkMessage = new StructNetworkMsg
            {
                MsgSender = PadString(MsgSender, 16).ToCharArray(),
                MsgDestination = PadString(MsgDestination, 16).ToCharArray(),
                MsgType = PadString(MsgType, 4).ToCharArray(),
                MsgID = PadString(MsgID, 4).ToCharArray(),
                MsgData = MsgData
            };
        }
        
        private void GetValuesFromStruct()
        {
            MsgSender = TrimString(_networkMessage.MsgSender);
            MsgDestination = TrimString(_networkMessage.MsgDestination);
            MsgType = TrimString(_networkMessage.MsgType);
            MsgID = TrimString(_networkMessage.MsgID);
            MsgData = _networkMessage.MsgData;
        }
        
        public byte[] ToByteArray()
        {
            CreateStruct();
            
            var buffer = new byte[StructSize];
            var h = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(_networkMessage, h.AddrOfPinnedObject(), false);
            }
            finally
            {
                h.Free();
            }
            return buffer;
        }
        
        public void GetMessageFromStructure(byte[] buffer)
        {
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                _networkMessage = (StructNetworkMsg)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(StructNetworkMsg));
                GetValuesFromStruct();
            }
            finally
            {
                handle.Free();
            }
        }
        
        private string PadString(string input, int length)
        {
            return input.PadRight(length, '\0');
        }
        
        private string TrimString(char[] input)
        {
            return new string(input).TrimEnd('\0');
        }
    }
}
