/*
 * Ported from Matthias L. Jugel's and Marcus Meiﬂner's JTA.
 *
 * (c) Matthias L. Jugel, Marcus Meiﬂner 1996-2002. All Rights Reserved.
 * (c) Seva Petrov 2002. All Rights Reserved.
 *
 * --LICENSE NOTICE--
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
 * --LICENSE NOTICE--
 *
 * $Date: 2003/05/30 06:10:54 $
 * $Id: TelnetWrapper.cs,v 1.3 2003/05/30 06:10:54 metaforge Exp $
 * 
 */

using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using Net.Graphite.Telnet;

namespace De.Mud.Telnet
{
	/// <summary>
	/// TelnetWrapper is a sample class demonstrating the use of the 
	/// telnet protocol handler.
	/// </summary>
	public class TelnetWrapper : TelnetProtocolHandler
	{

		#region Globals and properties


		// ManualResetEvent instances signal completion.
		private ManualResetEvent connectDone = new ManualResetEvent(false);
		private ManualResetEvent sendDone    = new ManualResetEvent(false);

		public event DisconnectedEventHandler Disconnected;
		public event DataAvailableEventHandler DataAvailable;

		private Socket socket;
        private string LastError;

		protected string hostname;
		protected int port;
	
		/// <summary>
		/// Sets the name of the host to connect to.
		/// </summary>
		public string Hostname
		{
			set
			{
				hostname = value;
			}
		}

		/// <summary>
		/// Sets the port on the remote host.
		/// </summary>
		public int Port
		{
			set
			{
				if (value > 0)
					port = value;
				else
					throw (new ArgumentException("Port number must be greater than 0.", "Port"));
			}
		}

		/// <summary>
		/// Sets the terminal width.
		/// </summary>
		public int TerminalWidth
		{
			set 
			{
				windowSize.Width = value;
			}
		}

		/// <summary>
		/// Sets the terminal height.
		/// </summary>
		public int TerminalHeight 
		{
			set 
			{
				windowSize.Height = value;
			}
		}

		/// <summary>
		/// Sets the terminal type.
		/// </summary>
		public string TerminalType
		{
			set 
			{
				terminalType = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether a connection to the remote
		/// resource exists.
		/// </summary>
		public bool Connected 
		{
			get 
			{
				return socket.Connected;
			}
		}

		#endregion

		#region Public interface

		/// <summary>
		/// Connects to the remote host  and opens the connection.
		/// </summary>
		public void Connect()
		{
			Connect(hostname, port);
		}

		/// <summary>
		/// Connects to the specified remote host on the specified port
		/// and opens the connection.
		/// </summary>
		/// <param name="host">Hostname of the Telnet server.</param>
		/// <param name="port">The Telnet port on the remote host.</param>
		public void Connect(string host, int port)
		{
			try
			{
				// Establish the remote endpoint for the socket.
				IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
				IPAddress ipAddress = ipHostInfo.AddressList[0];
				IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

				//  Create a TCP/IP  socket.
				socket = new Socket(AddressFamily.InterNetwork,
					SocketType.Stream, ProtocolType.Tcp);

				// Connect to the remote endpoint.
				socket.BeginConnect(remoteEP, 
					new AsyncCallback(ConnectCallback), socket);
				connectDone.WaitOne();
				Reset();
                // Clear LastError
                LastError = "";
			}
			catch (Exception e)
			{
                LastError = e.Message;
				Disconnect();
                throw;
            }
		}
  
		/// <summary>
		/// Sends a command to the remote host. A newline is appended.
		/// </summary>
		/// <param name="cmd">the command</param>
		/// <returns>output of the command</returns>
		public string Send(string cmd)
		{
			try 
			{
				byte[] arr = Encoding.Default.GetBytes(cmd);
				Transpose(arr);
				return null;
			} 
			catch (Exception e)
			{
                LastError = e.Message;
				Disconnect();
                return null;
//				throw(new ApplicationException("Error writing to socket.", e));
                
			}
		}

		/// <summary>
		/// Starts receiving data.
		/// </summary>
		public void Receive()
		{
			Receive(socket);
		}

		/// <summary>
		/// Disconnects the socket and closes the connection.
		/// </summary>
		public void Disconnect()
		{
			if (socket != null && socket.Connected)
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
                // send the Disconnected event to parent including a LastError messag, if one
                Disconnected(this, new TelnetWrapperDisconnctedEventArgs(LastError));
			}
		}

		#endregion

		#region IO methods

		/// <summary>
		/// Writes data to the socket.
		/// </summary>
		/// <param name="b">the buffer to be written</param>
		protected override void Write(byte[] b) 
		{
			if (socket.Connected)
				Send(socket, b);
			sendDone.WaitOne();
		}

		/// <summary>
		/// Callback for the connect operation.
		/// </summary>
		/// <param name="ar">Stores state information for this asynchronous 
		/// operation as well as any user-defined data.</param>
		private void ConnectCallback(IAsyncResult ar) 
		{
			Socket client = null;
			
			try 
			{
				// Retrieve the socket from the state object.
				client = (Socket)ar.AsyncState;

				// Complete the connection.
				client.EndConnect(ar);

				// Signal that the connection has been made.
				connectDone.Set();
			} 
			catch (Exception e) 
			{
                LastError = e.Message;
				Disconnect();
//                throw(new ApplicationException("Unable to connect to " + client.RemoteEndPoint.ToString(), e));
			}
		}

		/// <summary>
		/// Begins receiving for the data coming from the socket.
		/// </summary>
		/// <param name="client">The socket to get data from.</param>
		private void Receive(Socket client) 
		{
			try 
			{
				// Create the state object.
				State state = new State();
				state.WorkSocket = client;

				// Begin receiving the data from the remote device.
				client.BeginReceive(state.Buffer, 0, State.BufferSize, 0,
					new AsyncCallback(ReceiveCallback), state);
			} 
			catch (Exception e) 
			{
                LastError = e.Message;
				Disconnect();
//				throw(new ApplicationException("Error on read from socket.", e));
			}
		}

		/// <summary>
		/// Callback for the receive operation.
		/// </summary>
		/// <param name="ar">Stores state information for this asynchronous 
		/// operation as well as any user-defined data.</param>
		private void ReceiveCallback(IAsyncResult ar) 
		{
			try 
			{
				// Retrieve the state object and the client socket 
				// from the async state object.
				State state = (State) ar.AsyncState;
				Socket client = state.WorkSocket;

				// Read data from the remote device.
				int bytesRead = client.EndReceive(ar);

				if (bytesRead > 0) 
				{
					InputFeed(state.Buffer, bytesRead);
					Negotiate(state.Buffer);

					// Notify the caller that we have data.
					DataAvailable(this, 
						new DataAvailableEventArgs(Encoding.Default.GetString(state.Buffer, 0, bytesRead)));
                    // Get the rest of the data.
					client.BeginReceive(state.Buffer, 0, State.BufferSize, 0,
						new AsyncCallback(ReceiveCallback), state);
				} 
				else 
				{
					// Raise an event here signalling completion
					Disconnect();
				}
			} 
			catch (Exception e) 
			{
                LastError = e.Message;
				Disconnect();
//				throw(new ApplicationException("Error reading from socket.", e));
			}
		}

		/// <summary>
		/// Writes data to the socket.
		/// </summary>
		/// <param name="client">The socket to write to.</param>
		/// <param name="byteData">The data to write.</param>
		private void Send(Socket client, byte[] byteData) 
		{
			// Begin sending the data to the remote device.
			client.BeginSend(byteData, 0, byteData.Length, 0,
				new AsyncCallback(SendCallback), client);
		}

		/// <summary>
		/// Callback for the send operation.
		/// </summary>
		/// <param name="ar">Stores state information for this asynchronous 
		/// operation as well as any user-defined data.</param>
		private void SendCallback(IAsyncResult ar) 
		{
			// Retrieve the socket from the state object.
			Socket client = (Socket) ar.AsyncState;

			// Complete sending the data to the remote device.
			int bytesSent = client.EndSend(ar);

			// Signal that all bytes have been sent.
			sendDone.Set();
		}

		#endregion

		protected override void SetLocalEcho(bool echo) {}

		protected override void NotifyEndOfRecord() {}

		#region Cleanup

		public void Close() 
		{
			Dispose();
		}

		public void Dispose() 
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}

		protected void Dispose(bool disposing) 
		{
			if (disposing)
				Disconnect();
		}

		~TelnetWrapper() 
		{
			Dispose(false);
		}

		#endregion
	}

    #region EventArgs

    public class TelnetWrapperDisconnctedEventArgs : System.EventArgs
    {

        private string message;

        public TelnetWrapperDisconnctedEventArgs(string m)
        {
            this.message = m;
        }

        public string Message
        {
            get { return message; }
        }
    } 
    #endregion

    #region Event handlers

    /// <summary>
	/// A delegate type for hooking up disconnect notifications.
	/// </summary>
	public delegate void DisconnectedEventHandler(object sender, TelnetWrapperDisconnctedEventArgs e);

	/// <summary>
	/// A delegate type for hooking up data available notifications.
	/// </summary>
	public delegate void DataAvailableEventHandler(object sender, DataAvailableEventArgs e);

	#endregion
}
