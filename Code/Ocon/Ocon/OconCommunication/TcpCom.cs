using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ocon.OconCommunication
{
    class TcpCom : IOconCom
    {
        private readonly IOconSerializer _serializer;
        public const int CommunicationPort = 2026;

        private readonly IPAddress _multicastAddress = IPAddress.Parse("224.5.6.7");
        public const int MulticastPort = 2025;

        private readonly Dictionary<IPEndPoint, IOconPeer> _peers = new Dictionary<IPEndPoint, IOconPeer>();

        public TcpCom(IOconSerializer serializer)
        {
            _serializer = serializer;
            Address = new TcpPeer {Id = Guid.NewGuid(), IpEndPoint = new IPEndPoint(Dns.GetHostAddresses(Dns.GetHostName()).Single(ip => ip.AddressFamily == AddressFamily.InterNetwork), CommunicationPort)};
        }


        public async void Listen()
        {
            var ipep = new IPEndPoint(IPAddress.Any, CommunicationPort);
            var listener = new TcpListener(ipep);
            listener.Start();

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                Task.Run(async () => 
                {
                    var recieved = await ReadStringFromStream(client.GetStream());
                    var msg = _serializer.Deserialize<IOconMessage>(recieved);
                    RecievedMessageEvent(msg, AddOrGetPeer((IPEndPoint) client.Client.RemoteEndPoint));
                }).Start();
            }
        }

        public async void Discover()
        {
            var client = new UdpClient(MulticastPort, AddressFamily.InterNetwork);

            client.JoinMulticastGroup(_multicastAddress);
            client.EnableBroadcast = true;
 
            while (true)
            {
                var recieved = await client.ReceiveAsync();
                var msg = _serializer.Deserialize<IOconMessage>(recieved.Buffer.GetString());
                RecievedMessageEvent(msg, AddOrGetPeer(recieved.RemoteEndPoint));
                 
            }
        }

        public IOconPeer AddOrGetPeer(IPEndPoint ipep)
        {
            if (_peers.ContainsKey(ipep)) return _peers[ipep];
            var peer = new TcpPeer {Id = Guid.NewGuid(), IpEndPoint = ipep};
            PeerDiscoveredEvent(peer);
            return peer;
        }

        private async Task<string> ReadStringFromStream(NetworkStream stream)
        {
            var sb = new StringBuilder();
            do
            {
                var buffer = new byte[1024];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                sb.Append(buffer);
            } 
            while (stream.DataAvailable);
            return sb.ToString();
        }

        public void Send(IOconMessage msg, IOconPeer reciever)
        {
            Send(_serializer.Serialize(msg).GetBytes(), reciever);
        }


        public void Send(byte[] msg, IOconPeer reciever)
        {
            var peer = reciever as TcpPeer;
            if (peer == null) throw new ArgumentException("Reciever must be of type TcpPeer");

            using (var client = new TcpClient())
            {
                client.Connect(peer.IpEndPoint);

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
                using (
                    var mSendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
                        ProtocolType.Udp))
                {
                    //Setting up socket to multicast
                    mSendSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                        new MulticastOption(_multicastAddress, localIp));
                    mSendSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive,
                        255);
                    mSendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress,
                        true);
                    mSendSocket.MulticastLoopback = true;
                    mSendSocket.Bind(new IPEndPoint(ipToUse, MulticastPort));

                    //Sending out the package
                    mSendSocket.SendTo(_serializer.Serialize(msg).GetBytes(), new IPEndPoint(_multicastAddress, MulticastPort));
                }
            }
        }

        public event RecievedEventHandler RecievedMessageEvent;
        public event PeerDiscoveredHandler PeerDiscoveredEvent;
        public IOconPeer Address { get; private set; }
    }

    public class TcpPeer : IOconPeer
    {
        public Guid Id { get; set; }
        public IPEndPoint IpEndPoint { get; set; }
    }
}
