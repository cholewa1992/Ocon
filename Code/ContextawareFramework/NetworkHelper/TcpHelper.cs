using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ContextawareFramework;
using Newtonsoft.Json;

namespace NetworkHelper
{
    public class TcpHelper
    {

        //Cancellation tokens to stop network action
        private CancellationTokenSource _stopBroadcastTokenSource;
        private CancellationTokenSource _stopListenTokenSource;
        private CancellationTokenSource _stopDiscoveryTokenSource;

        public static bool IsListening { get; private set; }
        public static bool IsBroadcasting { get; private set; }

        //Settings
        public static readonly IPAddress StandardMulticastAddress = IPAddress.Parse("224.5.6.7");


        private static TcpHelper _instance;

        /// <summary>
        /// Constructs a new instance, or returns an active instance of the TcpHelper 
        /// </summary>
        /// <returns></returns>
        public static TcpHelper GetInstance()
        {
            return _instance ?? (_instance = new TcpHelper());
        }

        private TcpHelper()
        {
            
        }

        /// <summary>
        /// For broadcasting discovery pacakge so that peers can be auto-discovered. This metode runs on a separate thread and can be stopped by calling StopBroadcast
        /// </summary>
        public void Broadcast(IPAddress multicastAddress, int port = 2001)
        {
            //If the service is already running return
            if (IsBroadcasting) return;
            IsBroadcasting = true;

            _stopBroadcastTokenSource = new CancellationTokenSource();


            #region Broadcaster
            //Start listener for incomming replies
            Task.Run(() =>
            {
                while (true)
                {
                    //Broadcasting on every network interface
                    foreach (var localIp in Dns.GetHostAddresses(Dns.GetHostName()).Where(i => i.AddressFamily == AddressFamily.InterNetwork))
                    {
                        var ipToUse = localIp;
                        using (var mSendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                        {
                            //Setting up socket to multicast
                            mSendSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddress, localIp));
                            mSendSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 255);
                            mSendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                            mSendSocket.MulticastLoopback = true;
                            mSendSocket.Bind(new IPEndPoint(ipToUse, port));

                            //Creating discovery package to sent out
                            var json = JsonConvert.SerializeObject(localIp.ToString());

                            //Sending out the package
                            mSendSocket.SendTo(json.GetBytes(), new IPEndPoint(multicastAddress, port));
                        }
                        Thread.Sleep(5000);
                    }
                    if (_stopBroadcastTokenSource.Token.IsCancellationRequested)
                    {
                        IsBroadcasting = false;
                        _stopBroadcastTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    
                }
            }, _stopBroadcastTokenSource.Token);
            #endregion
        }

        /// <summary>
        /// Stops the broadcasting
        /// </summary>
        public void StopBroadcast()
        {
           _stopBroadcastTokenSource.Cancel();
        }

        /// <summary>
        /// This event will be fired when ever a new TCP package is avalible
        /// </summary>
        public event EventHandler<IncommingPackageEventArgs> IncommingTcpEvent;

        /// <summary>
        /// Starts the TCP listener
        /// </summary>
        public void StartListen(int bufferSize = 32, int port = 2002)
        {
            if (IsListening) return;
            IsListening = true;

            _stopListenTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                var localEndPoint = new IPEndPoint(IPAddress.Any, port);
                var tcpListener = new TcpListener(localEndPoint);
                tcpListener.Start();

                while (true)
                {
                    var client = tcpListener.AcceptTcpClient();
                    var stream = client.GetStream();

                    var msg = new List<byte>();

                    while (stream.DataAvailable)
                    {
                        var buffer = new byte[bufferSize];
                        stream.Read(buffer, 0, bufferSize);
                        msg.AddRange(buffer);
                    }

                    

                    if (IncommingTcpEvent != null)
                    {
                        IncommingTcpEvent(null, new IncommingPackageEventArgs(msg.GetString()));
                    }

                    if (_stopListenTokenSource.Token.IsCancellationRequested)
                    {
                        client.Close();
                        _stopListenTokenSource.Token.ThrowIfCancellationRequested();
                        IsListening = false;
                    }
                }
            }, _stopListenTokenSource.Token);
        }

        /// <summary>
        /// Stops the TCP listener
        /// </summary>
        public void StopListen()
        {
            _stopListenTokenSource.Cancel();
        }

        /// <summary>
        /// This event will be fired everytime a new widget is discovered. NB: StartDiscoveryService must be called to start the discovery service
        /// </summary>
        public event EventHandler<ContextFilterEventArgs> DiscoveryServiceEvent;

        /// <summary>
        /// Starts the Widget Discovery Service
        /// </summary>
        public void DiscoveryService(Guid guid, IPAddress multiCastAddress, int multiCastPort = 2001, int handShakePort = 2002)
        {
            _stopDiscoveryTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                try
                {
                    #region Socket Setup

                    //Constructs a new socket
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    //Binds the socket to listen on all network interfaces on port 2001
                    var ipep = new IPEndPoint(IPAddress.Any, multiCastPort);
                    socket.Bind(ipep);

                    //Set up the socket to listen for multicast messages on IP 224.5.6.7
                    var ip = multiCastAddress;
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));

                    #endregion

                    while (!_stopDiscoveryTokenSource.Token.IsCancellationRequested)
                    {
                        //Creates a buffer for receiving the broadcast packages 
                        var buffer = new byte[512];
                        socket.Receive(buffer);

                        //Getting data
                        IPAddress ipAddress;
                        IPAddress.TryParse(JsonConvert.DeserializeObject<string>(buffer.GetString()), out ipAddress);

                        var client = new TcpClient();
                        var serverEndPoint = new IPEndPoint(ipAddress, handShakePort);
                        client.Connect(serverEndPoint);

                        var clientStream = client.GetStream();
                        var msg = JsonConvert.SerializeObject(guid).GetBytes();

                        clientStream.Write(msg, 0, msg.Length);
                        clientStream.Flush();
                        client.Close();

                        //Fires an event about newly discovered context filter
                        if (DiscoveryServiceEvent != null)
                        {
                            DiscoveryServiceEvent(null, new ContextFilterEventArgs(new IPEndPoint(ipAddress, handShakePort)));
                        }
                    }
                    socket.Close();
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Discovery service has stopped due to following error: " + e.Message);
                }

            }, _stopDiscoveryTokenSource.Token);
        }

        /// <summary>
        /// Sends a TCP package
        /// </summary>
        /// <param name="msg">The message to transmit</param>
        /// <param name="ipep">The distination endpoint</param>
        public void SendPackage(string msg, IPEndPoint ipep)
        {
            // TODO check at den max er 1024 byte

            var client = new TcpClient();
            var serverEndPoint = ipep;
            client.Connect(serverEndPoint);

            var clientStream = client.GetStream();

            var bytes = msg.GetBytes();
            if(bytes.Length > 1024) throw new InvalidOperationException("");

            clientStream.Write(bytes, 0, bytes.Length);
            clientStream.Flush();
            client.Close();
        }
    }
    #region Helper classes
    /// <summary>
    /// Custom EventArgs to use for notification about new Widgets
    /// </summary>
    public class ContextFilterEventArgs : EventArgs
    {
        public Peer Peer { get; set; }

        internal ContextFilterEventArgs(IPEndPoint ipep)
        {
            Peer = new Peer { IpEndPoint = ipep };
        }
    }
    public class IncommingPackageEventArgs : EventArgs
    {
        public string Message { set; get; }

        internal IncommingPackageEventArgs(string msg)
        {
            Message = msg;
        }
    }
    #endregion
}