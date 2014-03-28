using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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
        private int _multicastPort = 2001;
        private int _communicationPort = 2002;

        private IPAddress _multicastAddress = IPAddress.Parse("224.5.6.7");
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
                while (true)
                {
                    //Broadcasting on every network interface
                    foreach (var localIp in Dns.GetHostAddresses(Dns.GetHostName()).Where(i => i.AddressFamily == AddressFamily.InterNetwork))
                    {
                        var ipToUse = localIp;
                        using (var mSendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                        {
                            //Setting up socket to multicast
                            mSendSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(MulticastAddress, localIp));
                            mSendSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 255);
                            mSendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                            mSendSocket.MulticastLoopback = true;
                            mSendSocket.Bind(new IPEndPoint(ipToUse, MulticastPort));

                            //Creating discovery package to sent out
                            var json = JsonConvert.SerializeObject(localIp.ToString());

                            //Sending out the package
                            mSendSocket.SendTo(json.GetBytes(), new IPEndPoint(MulticastAddress, MulticastPort));
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
        /// This event will be fired whenever a new client is avalible
        /// </summary>
        public event EventHandler<IncommingClientEventArgs> IncommingClient;

        /// <summary>
        /// This event will be fired whenever a new situation is avalible
        /// </summary>
        public event EventHandler<IncommingSituationEventArgs> IncommingSituationEvent;

        /// <summary>
        /// This event will be fired whenever a new entity is avalible
        /// </summary>
        public event EventHandler<IncommingEntityEventArgs> IncommingEntityEvent;

        /// <summary>
        /// Starts the TCP listener
        /// </summary>
        public void StartListen()
        {
            if (_stopListenTokenSource == null || _stopListenTokenSource.IsCancellationRequested)
                _stopListenTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                var localEndPoint = new IPEndPoint(IPAddress.Any, CommunicationPort);
                var tcpListener = new TcpListener(localEndPoint);
                tcpListener.Start();

                while (true)
                {
                    var client = tcpListener.AcceptTcpClient();
                    var stream = client.GetStream();

                    var header = new byte[1];
                    stream.Read(header, 0, 1);
                    var type = JsonConvert.DeserializeObject<PackageType>(header.GetString());


                    if (type == PackageType.Stream)
                    {
                        //If no one is subsribing, continue
                        if (IncommingSituationEvent == null) continue;

                        //Deserializing the ISituation
                        var situation = (ISituation) new BinaryFormatter().Deserialize(stream);

                        //Firering an Situation event 
                        IncommingSituationEvent(client.Client.RemoteEndPoint, new IncommingSituationEventArgs(situation));
                    }

                    else if (type == PackageType.String)
                    {
                        //If no one is subsribing, continue
                        if (IncommingEntityEvent == null) continue;

                        try
                        {
                            //Getting json string
                            var json = ReadStringFromStream(stream);

                            //Getting entity type from string
                            var entityType = Type.GetType(GetEntityTypeString(json));

                            //Getting entity from JSON
                            var entity = (IEntity) JsonConvert.DeserializeObject(json, entityType);

                            //Fireing Entity event
                            IncommingEntityEvent(client.Client.RemoteEndPoint, new IncommingEntityEventArgs(entity));
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

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
                    else
                    {
                        Console.WriteLine("Got an wired message from " + client.Client.LocalEndPoint);
                        Console.WriteLine(ReadStringFromStream(stream));
                    }

                    if (_stopListenTokenSource.Token.IsCancellationRequested)
                    {
                        client.Close();
                        _stopListenTokenSource.Token.ThrowIfCancellationRequested();
                    }
                }
            },
                _stopListenTokenSource.Token);
        }
        #region Listener helper methodes
        private string ReadStringFromStream(NetworkStream stream)
        {
            var msg = new List<byte>();
            while (!stream.DataAvailable) ;
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
                    #region Socket Setup
                    //Constructs a new socket
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    //Binds the socket to listen on all network interfaces on port 2001
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
                        IPAddress ipAddress;
                        IPAddress.TryParse(JsonConvert.DeserializeObject<string>(buffer.GetString()), out ipAddress);

                        var serverEndPoint = new IPEndPoint(ipAddress, CommunicationPort);
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
                    Console.WriteLine("Discovery service has stopped due to following error: " + e.Message);
                }

            }, _stopDiscoveryTokenSource.Token);
        }


        /// <summary>
        /// Method for sending an entity
        /// </summary>
        /// <param name="entity">The entity to send</param>
        /// <param name="ipep">The remote endpoint</param>
        public void SendEntity(IEntity entity, IPEndPoint ipep)
        {
            var json = JsonConvert.SerializeObject(entity, entity.GetType(),new JsonSerializerSettings{TypeNameHandling = TypeNameHandling.Objects});
            SendString(json, PackageType.String, ipep);
        }

        /// <summary>
        /// Method for sending an Situation
        /// </summary>
        /// <param name="situation">The situation to send</param>
        /// <param name="ipep">The remote endpoint</param>
        public void SendSituation(ISituation situation, IPEndPoint ipep)
        {
            var stream = new MemoryStream();
            new BinaryFormatter().Serialize(stream, situation);
            SendStream(stream, ipep);
        }


        /// <summary>
        /// Send from a source stream
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <param name="ipep">The IPEndpoint</param>
        private void SendStream(Stream stream, IPEndPoint ipep)
        {
            var client = new TcpClient();
            var serverEndPoint = ipep;
            client.Connect(serverEndPoint);

            var clientStream = client.GetStream();

            //Sending header
            var header = JsonConvert.SerializeObject(PackageType.Stream).GetBytes();
            clientStream.Write(header, 0, header.Length);

            //Sending message
            stream.CopyTo(clientStream);

            clientStream.Flush();
            client.Close();
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
    }

    public enum PackageType
    {
        Handshake = 1, String = 2, Stream = 3
    }
}