using System;
using System.Net;

namespace ContextawareFramework.NetworkHelper
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
        /// This event will be fired whenever a new client is avalible
        /// </summary>
        event EventHandler<IncommingClientEventArgs> IncommingClient;

        /// <summary>
        /// This event will be fired whenever a client subscribes a situation
        /// </summary>
        event EventHandler<IncommingSituationSubscribtionEventArgs> IncommingSituationSubscribtionEvent;

        /// <summary>
        /// This event will be fired whenever a new entity is avalible
        /// </summary>
        event EventHandler<IncommingEntityEventArgs> IncommingEntityEvent;

        /// <summary>
        /// This event will be fired whenever a situations state changes
        /// </summary>
        event EventHandler<IncommingSituationChangedEventArgs> IncommingSituationChangedEvent;

        /// <summary>
        /// Starts the listener
        /// </summary>
        void StartListen();

        /// <summary>
        /// Stops the listener
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
        /// Method for sending an entity
        /// </summary>
        /// <param name="entity">The entity to send</param>
        /// <param name="ipep">The remote endpoint</param>
        void SendEntity(IEntity entity, IPEndPoint ipep);

        /// <summary>
        /// Method for sending an Situation state to client
        /// </summary>
        /// <param name="situation">The situation whoms state to send</param>
        /// <param name="ipep">The remote endpoint</param>
        void SendSituationState(ISituation situation, IPEndPoint ipep);

        /// <summary>
        /// Method for subscribing to a Situation.
        /// </summary>
        /// <param name="guid">The clients GUID</param>
        /// <param name="situationIdentifier">The situation's identifier whom to subscribe</param>
        /// <param name="ipep">The remote endpoint</param>
        void SubscribeSituation(Guid guid, string situationIdentifier, IPEndPoint ipep);
    }
}