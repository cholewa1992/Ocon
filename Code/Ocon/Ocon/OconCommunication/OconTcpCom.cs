using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ocon.Entity;
using Ocon.Helper;

namespace Ocon.OconCommunication
{
    public class OconTcpCom : IOconCom
    {
        private readonly Peer _me;

        private readonly Dictionary<Peer, IPEndPoint> _peers =
            new Dictionary<Peer, IPEndPoint>(new PeerEquallityCompare());

        #region Cancellation tokens

        private CancellationTokenSource _stopBroadcastTokenSource;
        private CancellationTokenSource _stopDiscoveryTokenSource;
        private CancellationTokenSource _stopListenTokenSource;

        #endregion

        #region Fields

        private readonly TextWriter _log;
        private int _communicationPort = 2026;

        private IPAddress _multicastAddress = IPAddress.Parse("224.5.6.7");
        private int _multicastPort = 2025;

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

        public OconTcpCom(TextWriter log = null)
        {
            _me = new Peer {Guid = Guid.NewGuid()};
            _log = log;
        }

        public Peer Me
        {
            get { return _me; }
        }

        /// <summary>
        ///     For broadcasting discovery pacakge so that peers can be auto-discovered. This metode runs on a separate thread and
        ///     can be stopped by calling StopBroadcast
        /// </summary>
        public void Broadcast(int frequency = 30)
        {
            //If the service is already running return
            if (IsBroadcasting) return;
            IsBroadcasting = true;
            _stopBroadcastTokenSource = new CancellationTokenSource();

            #region Broadcaster

            //Start listener for incomming replies
            Task.Run(() =>
            {
                Logger.Write(_log, "Broadcast was started on " + MulticastAddress + ":" + MulticastPort);
                while (true)
                {
                    try
                    {
                        //Broadcasting on every network interface
                        foreach (
                            IPAddress localIp in
                                Dns.GetHostAddresses(Dns.GetHostName())
                                    .Where(i => i.AddressFamily == AddressFamily.InterNetwork))
                        {
                            IPAddress ipToUse = localIp;
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
                                string json =
                                    JsonConvert.SerializeObject(
                                        new Handshake {Peer = _me, Ip = localIp.ToString(), Port = CommunicationPort});

                                //Sending out the package
                                mSendSocket.SendTo(json.GetBytes(), new IPEndPoint(MulticastAddress, MulticastPort));
                            }
                            Thread.Sleep(frequency * 1000);
                        }
                        if (_stopBroadcastTokenSource.Token.IsCancellationRequested)
                        {
                            IsBroadcasting = false;
                            Logger.Write(_log, "Broadcast was stopped");
                            _stopBroadcastTokenSource.Token.ThrowIfCancellationRequested();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Write(_log, "Broadcast could not be made " + e.Message);
                    }
                }
            }, _stopBroadcastTokenSource.Token);

            #endregion
        }

        /// <summary>
        ///     Stops the broadcasting
        /// </summary>
        public void StopBroadcast()
        {
            Logger.Write(_log, "Broadcast is requested cancelled");

            _stopBroadcastTokenSource.Cancel();
        }

        //public event EventHandler<IncommingClientEventArgs> IncommingClient;
        /// <summary>
        ///     This event will be fired whenever a new client is avalible
        /// </summary>
        /// <summary>
        ///     This event will be fired whenever a new situation is avalible
        /// </summary>
        public event EventHandler<IncommingSituationSubscribtionEventArgs> IncommingSituationSubscribtionEvent;

        /// <summary>
        ///     This event will be fired whenever a new entity is avalible
        /// </summary>
        public event EventHandler<IncommingEntityEventArgs> IncommingEntityEvent;


        /// <summary>
        ///     This event will be fired whenever a situations state changes
        /// </summary>
        public event EventHandler<IncommingSituationChangedEventArgs> IncommingSituationChangedEvent;

        /// <summary>
        ///     Starts the TCP listener
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


                    Logger.Write(_log, "Started listening on port " + CommunicationPort);

                    while (true)
                    {
                        try
                        {
                            //Accepting client and starting stream
                            TcpClient client = tcpListener.AcceptTcpClient();

                            client.GetStream();

                            //Getting and deserializing message header
                            var message =
                                JsonConvert.DeserializeObject<Message>(ReadStringFromStream(client.GetStream()).Result);

                            //Adding peer to _peers
                            AddIpep(message.Peer,
                                new IPEndPoint(((IPEndPoint) client.Client.RemoteEndPoint).Address, CommunicationPort));
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
                            Logger.Write(_log, "An error occurred: " + e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Write(_log, "The listener stop due to an unrecoverble error: " + e.Message);
                }
                Logger.Write(_log, "Listening has stopped");
            },
                _stopListenTokenSource.Token);
        }


        /// <summary>
        ///     Stops the TCP listener
        /// </summary>
        public void StopListen()
        {
            Logger.Write(_log, "Listening is requested cancelled");
            _stopListenTokenSource.Cancel();
        }

        /// <summary>
        ///     This event will be fired everytime a new widget is discovered. NB: StartDiscoveryService must be called to start
        ///     the discovery service
        /// </summary>
        public event EventHandler<ContextFilterEventArgs> DiscoveryServiceEvent;

        /// <summary>
        ///     Starts the Widget Discovery Service
        /// </summary>
        public void DiscoveryService()
        {
            _stopDiscoveryTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                try
                {
                    Logger.Write(_log, "Discovery service started");

                    #region Socket Setup

                    //Constructs a new socket
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    //Binds the socket to listen on all network interfaces on port 2025
                    var ipep = new IPEndPoint(IPAddress.Any, MulticastPort);
                    socket.Bind(ipep);

                    //Set up the socket to listen for multicast messages on IP 224.5.6.7
                    IPAddress ip = MulticastAddress;
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
                        var remoteEndPoint = new IPEndPoint(IPAddress.Parse(data.Ip), data.Port);

                        if (!_peers.ContainsKey(data.Peer))
                        {
                            AddIpep(data.Peer, remoteEndPoint);

                            //Fires an event about newly discovered context filter
                            if (DiscoveryServiceEvent != null)
                            {
                                DiscoveryServiceEvent(null, new ContextFilterEventArgs(data.Peer));
                            }
                        }
                    }
                    socket.Close();
                }
                catch (SocketException e)
                {
                    Logger.Write(_log, "Discovery service has stopped due to following error: " + e.Message);
                }
                Logger.Write(_log, "Discovery service stopped");
            }, _stopDiscoveryTokenSource.Token);
        }

        /// <summary>
        ///     Method for subscribing to a Situation.
        /// </summary>
        /// <param name="situationName">The situation's identifier whom to subscribe</param>
        /// <param name="peer">The distination peer</param>
        public void SubscribeSituation(string situationName, Peer peer)
        {
            SendString(situationName, PackageType.SituationSubscription, peer);
        }

        /// <summary>
        ///     Method for sending an entity
        /// </summary>
        /// <param name="entity">The entity to send</param>
        /// <param name="peer">The distination peer</param>
        public void SendEntity(IEntity entity, Peer peer)
        {
            string json = JsonConvert.SerializeObject(entity,
                new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects});
            SendString(json, PackageType.Entity, peer);
        }

        /// <summary>
        ///     Method for sending an Situation state to client
        /// </summary>
        /// <param name="situation">The situation whoms state to send</param>
        /// <param name="peer">The distination peer</param>
        public void SendSituationState(Situation situation, Peer peer)
        {
            string json = JsonConvert.SerializeObject(new
            {
                SituationId = situation.Id,
                situation.State,
                SituationName = situation.Name
            });
            SendString(json, PackageType.SituationUpdate, peer);
        }

        private void Parse(Message message)
        {
            if (message.Type == PackageType.Entity)
            {
                //If no one is subsribing, continue
                if (IncommingEntityEvent == null) return;

                //Getting entity type from string
                Type entityType = Type.GetType(GetEntityTypeString(message.Body));

                //Getting entity from JSON
                var entity = (IEntity) JsonConvert.DeserializeObject(message.Body, entityType);

                //Fireing Entity event
                IncommingEntityEvent(message.Peer, new IncommingEntityEventArgs(entity));
            }
            else if (message.Type == PackageType.SituationUpdate)
            {
                //Taking entity and stores it as dynamic
                dynamic situationUpdate = JsonConvert.DeserializeObject(message.Body);

                //Fireing event
                var eventArgs = new IncommingSituationChangedEventArgs((Guid) situationUpdate.SituationId,
                    (bool) situationUpdate.State, (string) situationUpdate.SituationName);
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
                Logger.Write(_log, "Got a wired message from " + message.Peer);
            }
        }

        /// <summary>
        ///     Sends a TCP package
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
                    IPEndPoint serverEndPoint = IpepLookup(peer);
                    client.Connect(serverEndPoint);

                    NetworkStream clientStream = client.GetStream();

                    //Sending message
                    byte[] bytes =
                        JsonConvert.SerializeObject(new Message {Type = type, Peer = _me, Body = msg}).GetBytes();
                    clientStream.Write(bytes, 0, bytes.Length);

                    clientStream.Flush();
                    client.Close();
                }
                catch (Exception e)
                {
                    Logger.Write(_log, e.ToString());
                }
            });
        }


        private void AddIpep(Peer peer, IPEndPoint ipep)
        {
            lock (_peers) if (!_peers.ContainsKey(peer)) _peers.Add(peer, ipep);
        }

        private IPEndPoint IpepLookup(Peer peer)
        {
            if (_peers.ContainsKey(peer))
            {
                return _peers[peer];
            }
            throw new InvalidOperationException("The peer is not valid");
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
    }

    #region Helperclasses

    internal struct Handshake
    {
        public Peer Peer { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
    }

    internal struct Message
    {
        public Peer Peer { set; get; }
        public PackageType Type { set; get; }
        public string Body { set; get; }
    }

    internal static class ByteExtentions
    {
        public static byte[] GetBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string GetString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes.CropBytes());
        }

        public static string GetString(this ICollection<byte> bytes)
        {
            return Encoding.UTF8.GetString(bytes.ToArray());
        }


        private static byte[] CropBytes(this byte[] bytes)
        {
            var newArray = new byte[bytes.Count(t => t != 0)];

            for (int i = 0; i < newArray.Length; i++)
            {
                newArray[i] = bytes[i];
            }
            return newArray;
        }
    }
    #endregion
}
