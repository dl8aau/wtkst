namespace WinTest.DXLog
{
    public class NetworkMsgProcessor
    {
        public object ProcessMessage(byte[] rxData, int msgSize)
        {
            switch (msgSize)
            {
                case 0:
                    //        SMALL
                    //        NetworkSmallMessage s_msg = new();
                    //        s_msg.GetMessageFromStructure(rxData);
                    //        return s_msg.MsgType switch { _ => null };
                    return null;
                
                default:
                    var msg = new NetworkMessage();
                    msg.GetMessageFromStructure(rxData);
                    switch (msg.MsgType)
                    {
                        case "STI":
                            var statusInfo = new NetworkMsgStatusInfo();
                            statusInfo.GetHeaderFromStructure(msg.MsgData);
                            return statusInfo;

                        case "QSO":
                            var qso = new DXQSO();
                            qso.GetHeaderFromStructure(msg.MsgData);
                            return qso;

                        case "SRP": // Result is ignored
                        case "SSQ": // Result is ignored
                            var msg_srp = new NetworkMsgSync();
                            msg_srp.GetHeaderFromStructure(msg.MsgData);
                            return msg_srp;
                    }

                    return null;

            }

        }

    }
}
