using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WinTest
{
    public class wtListener
    {
        private volatile bool Listen;

        // problem: we need to use the UDPClient from Wintest.cs, as we cannot have the same port open twice
        public wtListener(int UDPPort)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, UDPPort);
            UdpClient u = new UdpClient();
            u.ExclusiveAddressUse = false;
            u.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            //u.Client.ReceiveTimeout = 1000;
            u.Client.Bind(ep);
            Listener(u);
        }

        public wtListener(UdpClient UdpClient)
        {
            Listener(UdpClient);
        }

        private void Listener(UdpClient u)
        { 
            Listen = true;

            Thread listener = new Thread(() =>
            {
                while (Listen == true)
                {
                    try
                    {
                        IPEndPoint ep = u.Client.LocalEndPoint as IPEndPoint;
                        byte[] data = u.Receive(ref ep);
                        if (data.Length > 0)
                        {
                            wtMessage Msg = new wtMessage(data);
                            //Console.WriteLine("WT Msg " + Msg.Msg + " src " + Msg.Src + " dest " + Msg.Dst + " data " + Msg.Data + " checksum " + Msg.HasChecksum);
                            if (wtMessageReceived != null)
                            {
                                wtMessageReceived(this, new wtMessageEventArgs(Msg));
                            }
                        }
                    }
                    catch (SocketException)
                    {
                        break; // exit the while loop
                    }
                }
                //Console.WriteLine("listener done");
            });
            listener.IsBackground = true;
            listener.Start();
        }

        public event EventHandler<wtMessageEventArgs> wtMessageReceived;
        public class wtMessageEventArgs : EventArgs
        {
            /// <summary>
            /// Win-Test message
            /// </summary>
            public wtMessageEventArgs(wtMessage Msg)
            {
                this.Msg = Msg;
            }
            public wtMessage Msg { get; private set; }
        }

        public void close()
        {
            Listen = false;
        }
    }
}
