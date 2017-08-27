using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WinTest
{
    public class wtListener
    {
        public const int WinTestDefaultPort = 9871;

        private volatile bool Listen;
        private UdpClient u;

        public wtListener(int UDPPort)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, UDPPort);
            u = new UdpClient();
            u.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            //u.Client.ReceiveTimeout = 1000;
            u.Client.Bind(ep);
            Listen = true;

            Thread listener = new Thread(() =>
            {
                while (Listen == true)
                {
                    try
                    {
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

            u.Client.Shutdown(SocketShutdown.Both);
            u.Close();
        }
    }
}
