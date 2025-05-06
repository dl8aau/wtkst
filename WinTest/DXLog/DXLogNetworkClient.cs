using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WinTest.DXLog
{
    public class DXLogNetworkClient
    {
        private const int Port = 9888;
        private UdpClient _udpClient;
        private int _messageId;

        private readonly Dictionary<string, NetworkMsgStatusInfo> _availableStations = new Dictionary<string, NetworkMsgStatusInfo>();

        public DXLogNetworkClient()
        {
            
        }

        public event EventHandler<DXQSO> DXLogQsoReceived;
        public event EventHandler<NetworkMsgStatusInfo> StationJoinedNetwork;
        public event EventHandler<string> StationLeftNetwork;
        
        public List<NetworkMsgStatusInfo> AvailableStations => _availableStations.Values.ToList();

        public async Task StartListeningAsync()
        {
            try
            {
                // Enable port sharing
                _udpClient = new UdpClient();
                _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, Port));

                SendSyncRequest();

                // Thread to listen for network messages.
                Task.Run(async () =>
                {
                    while (true)
                    {
                        var msg = await _udpClient.ReceiveAsync();
                        ProcessNetworkMessage(msg.Buffer);
                    }
                });
                
                // Thread to clean out stations no longer on the network
                Task.Run(() =>
                {
                    while (true)
                    {
                        foreach (var station in _availableStations.Where(s => (DateTime.UtcNow - s.Value.LastSeenUtc).TotalSeconds >= 30).ToList())
                        {
                            // If a station has not been seen for 30 seconds, we remove them from the available list and fire an event.
                            StationLeftNetwork?.Invoke(this, station.Key);
                            _availableStations.Remove(station.Key);
                        }

                        // Add 30s delay to avoid tight loop.
                        Task.Delay(30000).Wait(); 
                    }
                });
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        public void StopListening()
        {
            _udpClient?.Close();
            _udpClient = null;
        }

        private int GetMessageId()
        {
            _messageId = _messageId % 9999 + 1;
            return _messageId;
        }

        private void SendSyncRequest(int startQso = 0)
        {
            var msgSyncRequest = new NetworkMsgSyncRequest
            {
                IDQSO = startQso.ToString(),
                SyncPhase = 0
            };

            var networkMessage = new NetworkMessage
            {
                MsgDestination = "ALL",
                MsgSender = "WTKST",
                MsgID = GetMessageId().ToString(),
                MsgType = "SRQ",
                MsgData = msgSyncRequest.ToByteArray()
            };

            var rawMessage = networkMessage.ToByteArray();

            _udpClient.Send(rawMessage, rawMessage.Length, new IPEndPoint(IPAddress.Broadcast, Port));

            //Console.WriteLine($"SENT SRQ {startQso}");

        }

        private void RequestQso(string qsoId, string originStnID)
        {
            var msgSyncRequest = new NetworkMsgSync
            {
                QSOs =
                {
                    [0] = new NetworkMsgSync.SyncQSODesc { IDQSO = qsoId },
                    [1] = new NetworkMsgSync.SyncQSODesc { IDQSO = "0" }
                }
            };

            var networkMessage = new NetworkMessage
            {
                MsgDestination = originStnID,
                MsgSender = "WTKST",
                MsgID = GetMessageId().ToString(),
                MsgType = "SSQ",
                MsgData = msgSyncRequest.ToByteArray()
            };

            var rawMessage = networkMessage.ToByteArray();

            _udpClient.Send(rawMessage, rawMessage.Length, new IPEndPoint(IPAddress.Broadcast, Port));
            //Console.WriteLine($"SENT SSQ {qsoId} to {originStnID}");
        }


        private void ProcessNetworkMessage(byte[] rxMessage)
        {
            try
            {
                var networkMessage = new NetworkMessage();
                var msgType = new byte[8];
                Array.Copy(rxMessage, 64, msgType, 0, 8);
                var msgTypeString = Encoding.Unicode.GetString(msgType).Replace("\0", "");

                var networkMsgProcessor = new NetworkMsgProcessor();
                switch (msgTypeString)
                {
                    case "XXX":
                        break;

                    default:
                        networkMessage.GetMessageFromStructure(rxMessage);
                        if (networkMessage.MsgSender != "WTKST")
                        {
                            switch (networkMessage.MsgType)
                            {
                                case "STI":
                                    {
                                        if (networkMsgProcessor.ProcessMessage(rxMessage, 1) is NetworkMsgStatusInfo status)
                                        {
                                            // Check if the station already exists in the dictionary.
                                            if (_availableStations.ContainsKey(status.StationID))
                                            {
                                                // Update the existing station's status.
                                                _availableStations[status.StationID] = status;
                                            }
                                            else
                                            {
                                                // Add the new station to the dictionary and trigger the event for a station joining the network.
                                                _availableStations[status.StationID] = status;
                                                StationJoinedNetwork?.Invoke(this, status);
                                            }
                                        }
                                        break;
                                    }
                                case "QSO":
                                    {
                                        if (networkMsgProcessor.ProcessMessage(rxMessage, 1) is DXQSO qso)
                                        {
                                            DXLogQsoReceived?.Invoke(this, qso);
                                        }
                                        break;
                                    }
                                case "SRP":
                                    {
                                        if (networkMsgProcessor.ProcessMessage(rxMessage, 1) is NetworkMsgSync message)
                                        {
                                            foreach (var qso in message.QSOs)
                                            {
                                                if (string.IsNullOrEmpty(qso.IDQSO) || qso.IDQSO == "0" || qso.IDQSO == "-1")
                                                {
                                                    return;
                                                }
                                                RequestQso(qso.IDQSO, qso.OriginStnID);
                                            }
                                        }
                                        break;
                                    }
                            }
                        }

                        break;
                }

            }

            catch
            {

            }
        }



    }
}
