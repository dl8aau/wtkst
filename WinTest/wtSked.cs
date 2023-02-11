using System;
using System.Net;
using System.Net.Sockets;

namespace WinTest
{
    public class wtSked
    {
        private IPAddress localbroadcastIP;

        public wtSked()
        {
            localbroadcastIP = WinTest.GetIpIFBroadcastAddress();
        }

        private const string my_wtname = "KST"; // FIXME!!!

        private void send(wtMessage Msg)
        {
            try
            {
                UdpClient client = new UdpClient();
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
                client.Client.ReceiveTimeout = 10000;
                IPEndPoint groupEp = new IPEndPoint(localbroadcastIP, WinTest.WinTestDefaultPort);
                client.Connect(groupEp);
                Console.WriteLine("send: " + Msg.Data);
                byte[] b = Msg.ToBytes();
                client.Send(b, b.Length);
                client.Close();
            }
            catch
            {
            }
        }
        /*
         * 'WT Msg LOCKSKED src wtKST dest  data wtKST checksum True
WT Msg UNLOCKSKED src wtKST dest  data wtKST checksum True
WT Msg STATUS src STN1 dest  data 0 12 0 0 1441124 1 0 1  1440400  checksum True
STATUS: from STN1 band 144MHz mode 0 freq 1441124 pass 1440400
WT Msg LOCKSKED src STN1 dest  data STN1 checksum True
WT Msg UPDATESKED src STN1 dest  data 1506630300 1440400 G3XDY 1506630600 1440400 12 1 G3XDY [JO02OB - 279\260] checksum True
WT Msg UNLOCKSKED src STN1 dest  data STN1 checksum True
WT Msg STATUS src STN1 dest  data 0 12 0 0 1441124 1 0 1  1440400  checksum True
STATUS: from STN1 band 144MHz mode 0 freq 1441124 pass 1440400
WT Msg LOCKSKED src STN1 dest  data STN1 checksum True
WT Msg DELETESKED src STN1 dest  data 1506630600 1440400 G3XDY checksum True
WT Msg UNLOCKSKED src STN1 dest  data STN1 checksum True

WT Msg LOCKSKED src STN1 dest  data STN1 checksum True
WT Msg ADDSKED src STN1 dest  data 1506630660 1441124 12 1 G3XDY [JO02OB - 279\260] AS in 2min checksum True
ich
WT Msg ADDSKED src wtKST dest  data 1506639652 1441424 12 0 DL8AAI [JO02OB - 279?] AS in 2min checksum True
WT Msg UNLOCKSKED src STN1 dest  data STN1 checksum True

*/
        public void send_locksked(string target_wt)
        {
            wtMessage Msg = new wtMessage(WTMESSAGES.LOCKSKED, my_wtname, "", "\"" + my_wtname + "\"");
            send(Msg);
        }

        public void send_unlocksked(string target_wt)
        {
            wtMessage Msg = new wtMessage(WTMESSAGES.UNLOCKSKED, my_wtname, "", "\"" + my_wtname + "\"");
            send(Msg);
        }

        public void send_addsked(string target_wt, DateTime t, uint qrg, WTBANDS band, WTMODE mode, string call, string notes)
        {
            wtMessage Msg = new wtMessage(WTMESSAGES.ADDSKED, my_wtname, "", string.Concat(new string[]
            {
                // the time stamp for Win-Test seems to be 1 minute off, so use 1.1.1970 00:01:00 as reference
                ((Int64)((t.ToUniversalTime() - new DateTime (1970, 1, 1, 0, 1, 0, DateTimeKind.Utc)).TotalSeconds)+60).ToString(),
                " ", (10*qrg).ToString(), " ", ((int)band).ToString(),
                " ", ((int)mode).ToString(), " \"", call, "\" \"", notes, "\""
            }));
            send(Msg);
        }

    }
}
