using System;
using System.Net;

namespace NetworkHelper
{
    public interface ICommunicationHelper
    {
        /// <summary>
        /// For broadcasting discovery pacakge so that peers can be auto-discovered. This metode runs on a separate thread and can be stopped by calling StopBroadcast
        /// </summary>
        void Broadcast(IPAddress multicastAddress, int port = 2001);

        /// <summary>
        /// Stops the broadcasting
        /// </summary>
        void StopBroadcast();

        /// <summary>
        /// This event will be fired when ever a new TCP package is avalible
        /// </summary>
        event EventHandler<IncommingPackageEventArgs> IncommingTcpEvent;

        /// <summary>
        /// Starts the TCP listener
        /// </summary>
        void StartListen(int bufferSize = 1024, int port = 2002);

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
        void DiscoveryService(Guid guid, IPAddress multiCastAddress, int multiCastPort = 2001, int handShakePort = 2002);

        /// <summary>
        /// Sends a TCP package
        /// </summary>
        /// <param name="msg">The message to transmit</param>
        /// <param name="ipep">The distination endpoint</param>
        void SendPackage(string msg, IPEndPoint ipep);
    }
}