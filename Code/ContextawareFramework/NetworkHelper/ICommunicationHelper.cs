using System;
using System.IO;
using System.Net;

namespace NetworkHelper
{
    public interface ICommunicationHelper
    {
        int MulticastPort { get; set; }
        int CommunicationPort { set; get; }
        IPAddress MulticastAddress { get; set; }

        /// <summary>
        /// For broadcasting discovery pacakge so that peers can be auto-discovered. This metode runs on a separate thread and can be stopped by calling StopBroadcast
        /// </summary>
        void Broadcast();

        /// <summary>
        /// Stops the broadcasting
        /// </summary>
        void StopBroadcast();

        /// <summary>
        /// This event will be fired when ever a new TCP package is avalible
        /// </summary>
        event EventHandler<HandshakeEventArgs> HandshakeEvent;

        event EventHandler<StreamEventArgs> IncommingStreamEvent;
        event EventHandler<StringEventArgs> IncommingStringEvent;

        /// <summary>
        /// Starts the TCP listener
        /// </summary>
        void StartListen();

        /// <summary>
        /// Stops the TCP listener
        /// </summary>
        void StopListen();

        /// <summary>
        /// This event will be fired everytime a new widget is discovered. NB: StartDiscoveryService must be called to start the discovery service
        /// </summary>
        event EventHandler<ContextFilterEventArgs> DiscoveryServiceEvent;

        /// <summary>
        /// Starts the Widget Discovery Service
        /// </summary>
        void DiscoveryService(Guid guid, bool sendHandshake = false);

        /// <summary>
        /// Send from a source stream
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <param name="type">The package type</param>
        /// <param name="ipep">The IPEndpoint</param>
        void Send(Stream stream, PackageType type, IPEndPoint ipep);

        /// <summary>
        /// Sends a TCP package
        /// </summary>
        /// <param name="msg">The message to transmit</param>
        /// <param name="type">The package type</param>
        /// <param name="ipep">The distination endpoint</param>
        void SendString(string msg, PackageType type,  IPEndPoint ipep);
    }

    public enum PackageType
    {
        Handshake = 1, Stream = 2, String = 3
    }
}