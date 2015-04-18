using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Ocon.Helpers;
using Ocon.Messages;
using Ocon.OconCommunication;
using Ocon.OconSerializer;

namespace Ocon.TcpCom
{
    public class TcpCom : IOconComClient
    {
        private readonly IOconSerializer _serializer;
        public const int CommunicationPort = 2026;

        private readonly IPAddress _multicastAddress = IPAddress.Parse("239.5.6.7");
        public const int MulticastPort = 2025;

        private readonly Dictionary<IPEndPoint, IOconPeer> _peers = new Dictionary<IPEndPoint, IOconPeer>();

        private UdpClient _udpClient;
        private TcpListener _tcpClient;


        public event RecievedEventHandler RecievedMessageEvent;
        public IOconPeer Address { get; private set; }

        public TcpCom(IOconSerializer serializer)
        {
            _serializer = serializer;
            Address = new Peer(Guid.NewGuid());
            Listen();
            BroadcastListen();
        }

        public async void Listen()
        {
            var ipep = new IPEndPoint(IPAddress.Any, CommunicationPort);
            _tcpClient = new TcpListener(ipep);
            _tcpClient.Start();

            while (true)
            {
                var client = await _tcpClient.AcceptTcpClientAsync();
                client.ReceiveTimeout = 5000;
                client.SendTimeout = 5000;
                Task.Run(async () => 
                {
                    using (client)
                    {
                        var recieved = await ReadStringFromStream(client.GetStream());
                        var msg = _serializer.Deserialize<Message>(recieved);
                        var endpoint = (IPEndPoint) client.Client.RemoteEndPoint;
                        if (RecievedMessageEvent != null)
                            RecievedMessageEvent(msg.Msg, AddOrGetPeer(endpoint, msg.Peer));
                    }
                });
            }
        }

        public async void BroadcastListen()
        {
            _udpClient = new UdpClient();

            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpClient.ExclusiveAddressUse = false;
            _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, MulticastPort));
            _udpClient.JoinMulticastGroup(_multicastAddress);
 
            while (true)
            {
                var recieved = await _udpClient.ReceiveAsync();
                var msg = _serializer.Deserialize<MulticastMsg>(recieved.Buffer.GetString());
                if(RecievedMessageEvent != null)
                    RecievedMessageEvent(msg.Msg, AddOrGetPeer(new IPEndPoint(new IPAddress(msg.Ip), CommunicationPort ), msg.Peer));
            }
        }

        private IOconPeer AddOrGetPeer(IPEndPoint ipep, IOconPeer peer = null)
        {
            ipep.Port = CommunicationPort;
            if (_peers.ContainsKey(ipep)) return _peers[ipep];
            peer = peer ?? new Peer(Guid.NewGuid()); 
            _peers.Add(ipep,peer);
            return peer;
        }

        private async Task<string> ReadStringFromStream(NetworkStream stream)
        {
            var bytes = new List<byte>();
            do
            {
                var buffer = new byte[4096];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                bytes.AddRange(buffer);
            } 
            while (stream.DataAvailable);
            return bytes.GetString();
        }

        public void Send(IOconMessage msg, IOconPeer reciever)
        {
            Send(_serializer.Serialize(new Message(msg, Address)).GetBytes(), reciever);
        }


        public void Send(byte[] msg, IOconPeer peer)
        {
            if (peer == null) throw new ArgumentException("Reciever must be of type Peer");

            using (var client = new TcpClient())
            {
                client.Connect(_peers.Single(t => t.Value.Equals(peer)).Key);
                using (var clientStream = client.GetStream())
                {
                    clientStream.Write(msg, 0, msg.Length);
                    clientStream.Flush();
                }
            }
        }

        public void Broadcast(IOconMessage msg)
        {
            foreach (
                var localIp in
                    Dns.GetHostAddresses(Dns.GetHostName())
                        .Where(i => i.AddressFamily == AddressFamily.InterNetwork))
            {
                var ipToUse = localIp;
                using ( var client = new UdpClient())

                {
                    client.MulticastLoopback = true;
                    client.JoinMulticastGroup(_multicastAddress);
                    var bytes = _serializer.Serialize(new MulticastMsg(msg, ipToUse.GetAddressBytes(), Address)).GetBytes();
                    client.Send(bytes, bytes.Length, new IPEndPoint(_multicastAddress, MulticastPort));
}
            }
        }

        #region Helper classes
        private class Message
        {
            public IOconMessage Msg { get; private set; }

            public IOconPeer Peer { get; private set; }

            public  Message(IOconMessage msg, IOconPeer peer)
            {
                Peer = peer;
                Msg = msg;
            }
        }

        private class MulticastMsg : Message
        {
            public byte[] Ip { get; private  set; }

            public  MulticastMsg(IOconMessage msg, byte[] ip, IOconPeer peer) : base(msg,peer)
            {
                Ip = ip;
            }
        }
        #endregion

        public void Dispose()
        {
            _tcpClient.Stop();
            _udpClient.Close();
        }
    }
}
