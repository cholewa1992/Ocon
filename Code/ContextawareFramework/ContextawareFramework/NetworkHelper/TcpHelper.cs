using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContextawareFramework.NetworkHelper
{
    public class TcpHelper : ICommunicationHelper
    {

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
        private TextWriter _outputStream;

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

        /// <summary>
        /// Constructs a new instance, or returns an active instance of the TcpHelper 
        /// </summary>
        /// <returns></returns>
        public static TcpHelper GetInstance(TextWriter errorLogWriter = null)
        {
            return _instance ?? (_instance = new TcpHelper{_outputStream = errorLogWriter});
        }

        private TcpHelper()
        {
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
                                    JsonConvert.SerializeObject(new {IPAddress = localIp.ToString(), CommunicationPort});

                                //Sending out the package
                                mSendSocket.SendTo(json.GetBytes(), new IPEndPoint(MulticastAddress, MulticastPort));
                            }
                            Thread.Sleep(60000);
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
        public event EventHandler<IncommingClientEventArgs> IncommingClient;

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
                            var stream = client.GetStream();

                            //Getting and deserializing message header
                            var header = new byte[1];
                            stream.Read(header, 0, 1);
                            var type = JsonConvert.DeserializeObject<PackageType>(header.GetString());


                            if (type == PackageType.Entity)
                            {
                                //If no one is subsribing, continue
                                if (IncommingEntityEvent == null) continue;

                                //Getting json string
                                var json = ReadStringFromStream(stream);

                                //Getting entity type from string
                                var entityType = Type.GetType(GetEntityTypeString(json));

                                //Getting entity from JSON
                                var entity = (IEntity) JsonConvert.DeserializeObject(json, entityType);

                                //Fireing Entity event
                                IncommingEntityEvent(client.Client.RemoteEndPoint, new IncommingEntityEventArgs(entity));
                            }
                            else if (type == PackageType.SituationUpdate)
                            {
                                //Getting JSON from stream
                                var json = ReadStringFromStream(stream);

                                //Taking entity and stores it as dynamic
                                dynamic situationUpdate = JsonConvert.DeserializeObject(json);

                                //Fireing event
                                var eventArgs = new IncommingSituationChangedEventArgs(situationUpdate.Guid,
                                    situationUpdate.State);
                                IncommingSituationChangedEvent(client.Client.RemoteEndPoint, eventArgs);
                            }

                            else if (type == PackageType.Handshake)
                            {
                                //If no one is subsribing, continue
                                if (IncommingClient == null) continue;

                                //Getting remote endpoint
                                var ripep = client.Client.RemoteEndPoint as IPEndPoint;

                                //If the remote endpoint is empty continue
                                if (ripep == null) continue;

                                //Getting guid from JSON
                                var guid = JsonConvert.DeserializeObject<Guid>(ReadStringFromStream(stream));

                                //Fireing Client event
                                IncommingClient(client.Client.RemoteEndPoint, new IncommingClientEventArgs(ripep, guid));
                            }
                            else if (type == PackageType.SituationSubscription)
                            {
                                //Getting json
                                dynamic subscription = JsonConvert.DeserializeObject(ReadStringFromStream(stream));

                                //Getting data and firing event
                                var eventArgs = new IncommingSituationSubscribtionEventArgs(subscription.Guid,
                                    subscription.SituationIdentifier);
                                IncommingSituationSubscribtionEvent(client.Client.RemoteEndPoint, eventArgs);
                            }
                            else
                            {
                                Log("Got an wired message from " + client.Client.LocalEndPoint);
                                Log(ReadStringFromStream(stream));
                            }

                            if (_stopListenTokenSource.Token.IsCancellationRequested)
                            {
                                client.Close();
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

        #region Listener helper methodes
        private string ReadStringFromStream(NetworkStream stream)
        {
            var msg = new List<byte>();
            while (!stream.DataAvailable);
            while (stream.DataAvailable)
            {
                var buffer = new byte[1024];
                stream.Read(buffer, 0, buffer.Length);
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
        public void DiscoveryService(Guid guid, bool sendHandshake = false)
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
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));
                    #endregion

                    while (!_stopDiscoveryTokenSource.Token.IsCancellationRequested)
                    {
                        //Creates a buffer for receiving the broadcast packages 
                        var buffer = new byte[512];
                        socket.Receive(buffer);

                        //Getting data


                        dynamic data = JsonConvert.DeserializeObject(buffer.GetString());
                        var serverEndPoint = new IPEndPoint(IPAddress.Parse(data.IPAddress.ToString()), (int) data.CommunicationPort );

                        var msg = JsonConvert.SerializeObject(guid);

                        if (sendHandshake)

                        {
                            SendString(msg, PackageType.Handshake, serverEndPoint);
                        }

                        //Fires an event about newly discovered context filter
                        if (DiscoveryServiceEvent != null)
                        {
                            DiscoveryServiceEvent(null, new ContextFilterEventArgs(serverEndPoint));
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
        /// <param name="guid">The clients GUID</param>
        /// <param name="situationIdentifier">The situation's identifier whom to subscribe</param>
        /// <param name="ipep">The remote endpoint</param>
        public void SubscribeSituation(Guid guid, string situationIdentifier, IPEndPoint ipep)
        {
            var json = JsonConvert.SerializeObject(new {Guid = guid, SituationIdentifier = situationIdentifier});

            SendString(situationIdentifier, PackageType.SituationSubscription, ipep);
        }

        /// <summary>
        /// Method for sending an entity
        /// </summary>
        /// <param name="entity">The entity to send</param>
        /// <param name="ipep">The remote endpoint</param>
        public void SendEntity(IEntity entity, IPEndPoint ipep)
        {
            var json = JsonConvert.SerializeObject(entity, entity.GetType(),new JsonSerializerSettings{TypeNameHandling = TypeNameHandling.Objects});
            SendString(json, PackageType.Entity, ipep);
        }

        /// <summary>
        /// Method for sending an Situation state to client
        /// </summary>
        /// <param name="situation">The situation whoms state to send</param>
        /// <param name="ipep">The remote endpoint</param>
        public void SendSituationState(ISituation situation, IPEndPoint ipep)
        {
            var json = JsonConvert.SerializeObject(new
            {
                SituationId = situation.SubscribersAddresse,
                State = situation.State
            });
            SendString(json, PackageType.SituationUpdate, ipep);
        }

        /// <summary>
        /// Sends a TCP package
        /// </summary>
        /// <param name="msg">The message to transmit</param>
        /// <param name="type">The package type</param>
        /// <param name="ipep">The distination endpoint</param>
        private void SendString(string msg, PackageType type,  IPEndPoint ipep)
        {
            var client = new TcpClient();
            var serverEndPoint = ipep;
            client.Connect(serverEndPoint);

            var clientStream = client.GetStream();

            //Sending header
            var header = JsonConvert.SerializeObject(type).GetBytes();
            clientStream.Write(header, 0, header.Length);

            //Sending message
            var bytes = msg.GetBytes();
            clientStream.Write(bytes, 0, bytes.Length);

            clientStream.Flush();
            client.Close();
 }

        /// <summary>
        /// Private method for making error log messages
        /// </summary>
        /// <param name="msg"></param>
        private void Log(string msg)
        {
            if(_outputStream != null) _outputStream.WriteLine(msg);
        }

    }
}