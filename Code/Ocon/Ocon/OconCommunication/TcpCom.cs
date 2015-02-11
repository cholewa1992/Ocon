using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using Ocon.Helper;

namespace Ocon.OconCommunication
{
    class TcpCom : IOconComNew
    {
        private const int CommunicationPort = 2026;

        private readonly IPAddress _multicastAddress = IPAddress.Parse("224.5.6.7");
        private const int MulticastPort = 2025;


        public void Send(string msg, IOconPeer reciever)
        {
            Send(msg.GetBytes(), reciever);
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

                    //Sending message
                    clientStream.Write(msg, 0, msg.Length);

                    clientStream.Flush();
                }
            }
        }

        public void Broadcast(string msg)
        {
            Broadcast(msg.GetBytes());
        }

        public void Broadcast(byte[] msg)
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
                    mSendSocket.SendTo(msg, new IPEndPoint(_multicastAddress, MulticastPort));
                }
            }
        }

        public event RecievedEventHandler RecievedMessageEvent;
        public event PeerDiscoveredHandler PeerDiscoveredEvent;
    }

    public class TcpPeer : IOconPeer
    {
        public Guid Id { get; set; }
        public IPEndPoint IpEndPoint { get; set; }
    }
}
