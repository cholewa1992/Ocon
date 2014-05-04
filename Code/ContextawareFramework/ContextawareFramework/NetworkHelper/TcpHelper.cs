using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContextawareFramework.NetworkHelper
{
    public class TcpHelper : ICommunicationHelper
    {
        private readonly Peer _me;

        #region Cancellation tokens

        private CancellationTokenSource _stopBroadcastTokenSource;
        private CancellationTokenSource _stopListenTokenSource;
        private CancellationTokenSource _stopDiscoveryTokenSource;

        #endregion

        #region Fields

        private static TcpHelper _instance;
        private int _multicastPort = 2025;
        private int _communicationPort = 2026;

        private IPAddress _multicastAddress = IPAddress.Parse("224.5.6.7");
        private readonly TextWriter _outputStream;

        #endregion

        #region Properties

        public static bool IsBroadcasting { get; private set; }

        public int MulticastPort
        {
            get { return _multicastPort; }
            set { _multicastPort = value; }
        }

        public int CommunicationPort
        {
            set { _communicationPort = value; }
            get { return _communicationPort; }
        }

        public IPAddress MulticastAddress
        {
            get { return _multicastAddress; }
            set { _multicastAddress = value; }
        }

        #endregion

        public Peer Me
        {
            get { return _me; }
        }

        public TcpHelper(TextWriter errorLogWriter = null)
        {
            _me = new Peer{Guid = Guid.NewGuid()};
            _outputStream = errorLogWriter;
        }

        /// <summary>
        /// For broadcasting discovery pacakge so that peers can be auto-discovered. This metode runs on a separate thread and can be stopped by calling StopBroadcast
        /// </summary>
        public void Broadcast()
        {
            //If the service is already running return
            if (IsBroadcasting) return;
            IsBroadcasting = true;
            _stopBroadcastTokenSource = new CancellationTokenSource();

            #region Broadcaster

            //Start listener for incomming replies
            Task.Run(() =>
            {
                Log("Broadcast was started on " + MulticastAddress + ":" + MulticastPort);
                while (true)
                {
                    try
                    {

                        //Broadcasting on every network interface
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
                                    new MulticastOption(MulticastAddress, localIp));
                                mSendSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive,
                                    255);
                                mSendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress,
                                    true);
                                mSendSocket.MulticastLoopback = true;
                                mSendSocket.Bind(new IPEndPoint(ipToUse, MulticastPort));

                                //Creating discovery package to sent out
                                var json =
                                    JsonConvert.SerializeObject(
                                        new Handshake { Peer = _me, Ip = localIp.ToString(), Port = CommunicationPort });

                                //Sending out the package
                                mSendSocket.SendTo(json.GetBytes(), new IPEndPoint(MulticastAddress, MulticastPort));
                            }
                            Thread.Sleep(5000);
                        }
                        if (_stopBroadcastTokenSource.Token.IsCancellationRequested)
                        {
                            IsBroadcasting = false;
                            Log("Broadcast was stopped");
                            _stopBroadcastTokenSource.Token.ThrowIfCancellationRequested();
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                        Log("Broadcast could not be made " + e.Message);
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
            Log("Broadcast is requested cancelled");

            _stopBroadcastTokenSource.Cancel();
        }

        /// <summary>
        /// This event will be fired whenever a new client is avalible
        /// </summary>
        //public event EventHandler<IncommingClientEventArgs> IncommingClient;

        /// <summary>
        /// This event will be fired whenever a new situation is avalible
        /// </summary>
        public event EventHandler<IncommingSituationSubscribtionEventArgs> IncommingSituationSubscribtionEvent;

        /// <summary>
        /// This event will be fired whenever a new entity is avalible
        /// </summary>
        public event EventHandler<IncommingEntityEventArgs> IncommingEntityEvent;


        /// <summary>
        /// This event will be fired whenever a situations state changes
        /// </summary>
        public event EventHandler<IncommingSituationChangedEventArgs> IncommingSituationChangedEvent;

        /// <summary>
        /// Starts the TCP listener
        /// </summary>
        public void StartListen()
        {
            if (_stopListenTokenSource == null || _stopListenTokenSource.IsCancellationRequested)
                _stopListenTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                try
                {
                    var localEndPoint = new IPEndPoint(IPAddress.Any, CommunicationPort);
                    var tcpListener = new TcpListener(localEndPoint);
                    tcpListener.Start();
                 

                    Log("Started listening on port " + CommunicationPort);

                    while (true)
                    {
                        try
                        {
                            //Accepting client and starting stream
                            var client = tcpListener.AcceptTcpClient();

                            client.GetStream();

                            //Getting and deserializing message header
                            var message = JsonConvert.DeserializeObject<Message>(ReadStringFromStream(client.GetStream()).Result);

                            //Adding peer to _peers
                            AddIpep(message.Peer, new IPEndPoint((client.Client.RemoteEndPoint as IPEndPoint).Address, CommunicationPort));
                            Parse(message);
                            
                            client.Close();

                            if (_stopListenTokenSource.Token.IsCancellationRequested)
                            {
                                tcpListener.Stop();
                                _stopListenTokenSource.Token.ThrowIfCancellationRequested();
                            }
                        }
                        catch (Exception e)
                        {
                            Log("An error occurred: " + e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log("The listener stop due to an unrecoverble error: " + e.Message);
                }
                Log("Listening has stopped");
            },
                _stopListenTokenSource.Token);
        }


        private void Parse(Message message)
        {
            if (message.Type == PackageType.Entity)
            {
                //If no one is subsribing, continue
                if (IncommingEntityEvent == null) return;

                //Getting entity type from string
                var entityType = Type.GetType(GetEntityTypeString(message.Body));

                //Getting entity from JSON
                var entity = (IEntity)JsonConvert.DeserializeObject(message.Body, entityType);

                //Fireing Entity event
                IncommingEntityEvent(message.Peer, new IncommingEntityEventArgs(entity));
            }
            else if (message.Type == PackageType.SituationUpdate)
            {
                //Taking entity and stores it as dynamic
                dynamic situationUpdate = JsonConvert.DeserializeObject(message.Body);

                //Fireing event
                var eventArgs = new IncommingSituationChangedEventArgs(situationUpdate.SituationId,
                    situationUpdate.State);
                IncommingSituationChangedEvent(message.Peer, eventArgs);
            }
            else if (message.Type == PackageType.SituationSubscription)
            {

                //Getting data and firing event
                var eventArgs = new IncommingSituationSubscribtionEventArgs(message.Peer, message.Body);
                IncommingSituationSubscribtionEvent(message.Peer, eventArgs);
            }
            else
            {
                Log("Got an wired message from " + message.Peer);
            }
        }

        #region Listener helper methodes

        private async Task<string> ReadStringFromStream(NetworkStream stream)
        {
            var msg = new List<byte>();
            while (!stream.DataAvailable) ;
            while (stream.DataAvailable)
            {
                var buffer = new byte[1024];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                msg.AddRange(buffer);
            }
            return msg.GetString();
        }

        private static string GetEntityTypeString(string json)
        {
            string typeString = JObject.Parse(json)["$type"].ToString();
            string[] trim = typeString.Split(',');
            return trim[0];
        }

        #endregion

        /// <summary>
        /// Stops the TCP listener
        /// </summary>
        public void StopListen()
        {
            Log("Listening is requested cancelled");
            _stopListenTokenSource.Cancel();
        }

        /// <summary>
        /// This event will be fired everytime a new widget is discovered. NB: StartDiscoveryService must be called to start the discovery service
        /// </summary>
        public event EventHandler<ContextFilterEventArgs> DiscoveryServiceEvent;

        /// <summary>
        /// Starts the Widget Discovery Service
        /// </summary>
        public void DiscoveryService()
        {
            _stopDiscoveryTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                try
                {
                    Log("Discovery service started");

                    #region Socket Setup

                    //Constructs a new socket
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    //Binds the socket to listen on all network interfaces on port 2025
                    var ipep = new IPEndPoint(IPAddress.Any, MulticastPort);
                    socket.Bind(ipep);

                    //Set up the socket to listen for multicast messages on IP 224.5.6.7
                    var ip = MulticastAddress;
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                        new MulticastOption(ip, IPAddress.Any));

                    #endregion

                    while (!_stopDiscoveryTokenSource.Token.IsCancellationRequested)
                    {
                        //Creates a buffer for receiving the broadcast packages 
                        var buffer = new byte[512];
                        socket.Receive(buffer);

                        //Getting data
                        var data = JsonConvert.DeserializeObject<Handshake>(buffer.GetString());
                        var remoteEndPoint = new IPEndPoint(IPAddress.Parse(data.Ip),data.Port);

                        AddIpep(data.Peer, remoteEndPoint);

                        //Fires an event about newly discovered context filter
                        if (DiscoveryServiceEvent != null)
                        {
                            DiscoveryServiceEvent(null, new ContextFilterEventArgs(data.Peer));
                        }
                    }
                    socket.Close();
                }
                catch (SocketException e)
                {
                    Log("Discovery service has stopped due to following error: " + e.Message);
                }
                Log("Discovery service stopped");
            }, _stopDiscoveryTokenSource.Token);
        }

        /// <summary>
        /// Method for subscribing to a Situation.
        /// </summary>
        /// <param name="situationIdentifier">The situation's identifier whom to subscribe</param>
        /// <param name="peer">The distination peer</param>
        public void SubscribeSituation(string situationIdentifier, Peer peer)
        {
            SendString(situationIdentifier, PackageType.SituationSubscription, peer);
        }

        /// <summary>
        /// Method for sending an entity
        /// </summary>
        /// <param name="entity">The entity to send</param>
        /// <param name="peer">The distination peer</param>
        public void SendEntity(IEntity entity, Peer peer)
        {
            var json = JsonConvert.SerializeObject(entity, entity.GetType(),
                new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects});
            SendString(json, PackageType.Entity, peer);
        }

        /// <summary>
        /// Method for sending an Situation state to client
        /// </summary>
        /// <param name="situation">The situation whoms state to send</param>
        /// <param name="peer">The distination peer</param>
        public void SendSituationState(ISituation situation, Peer peer)
        {
            var json = JsonConvert.SerializeObject(new
            {
                SituationId = situation.Id,
                State = situation.State
            });
            SendString(json, PackageType.SituationUpdate, peer);
        }

        /// <summary>
        /// Sends a TCP package
        /// </summary>
        /// <param name="msg">The message to transmit</param>
        /// <param name="type">The package type</param>
        /// <param name="peer">The distination peer</param>
        private void SendString(string msg, PackageType type, Peer peer)
        {
            Task.Run(() =>
            {
                try
                {
                    var client = new TcpClient();
                    var serverEndPoint = IpepLookup(peer);
                    client.Connect(serverEndPoint);

                    var clientStream = client.GetStream();

                    //Sending message
                    var bytes = JsonConvert.SerializeObject(new Message {Type = type, Peer = _me, Body = msg}).GetBytes();

                    Console.WriteLine(bytes.Length);

                    clientStream.Write(bytes, 0, bytes.Length);

                    clientStream.Flush();
                    client.Close();
                }
                catch (Exception e)
                {
                    Log(e.Message);
                }
            });
        }

        /// <summary>
        /// Private method for making error log messages
        /// </summary>
        /// <param name="msg"></param>
        private void Log(string msg)
        {
            if (_outputStream != null) _outputStream.WriteLine(msg);
        }


        readonly Dictionary<Peer, IPEndPoint> _peers = new Dictionary<Peer, IPEndPoint>(new PeerEquallityCompare());

        private void AddIpep(Peer peer, IPEndPoint ipep)
        {
            lock(_peers) if(!_peers.ContainsKey(peer)) _peers.Add(peer,ipep);
        }

        private IPEndPoint IpepLookup(Peer peer)
        {
            if (_peers.ContainsKey(peer))
            {
                return _peers[peer];
            }
            throw new InvalidOperationException("The peer is not valid");
        }
    }

    public struct Handshake
    {
        public Peer Peer { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
    }

    public struct Message
    {
        public Peer Peer { set; get; }
        public PackageType Type { set; get; }
        public string Body { set; get; }
    }
}