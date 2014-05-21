using System;
using Ocon.Entity;

namespace Ocon.OconCommunication
{
    public interface IOconCom
    {
        Peer Me { get; }

        /// <summary>
        ///     For broadcasting discovery pacakge so that peers can be auto-discovered. This metode runs on a separate thread and
        ///     can be stopped by calling StopBroadcast
        /// </summary>
        void Broadcast(int frequency = 30);

        /// <summary>
        ///     Stops the broadcasting
        /// </summary>
        void StopBroadcast();

        //public event EventHandler<IncommingClientEventArgs> IncommingClient;
        /// <summary>
        ///     This event will be fired whenever a new client is avalible
        /// </summary>
        /// <summary>
        ///     This event will be fired whenever a new situation is avalible
        /// </summary>
        event EventHandler<IncommingSituationSubscribtionEventArgs> IncommingSituationSubscribtionEvent;

        /// <summary>
        ///     This event will be fired whenever a new entity is avalible
        /// </summary>
        event EventHandler<IncommingEntityEventArgs> IncommingEntityEvent;

        /// <summary>
        ///     This event will be fired whenever a situations state changes
        /// </summary>
        event EventHandler<IncommingSituationChangedEventArgs> IncommingSituationChangedEvent;

        /// <summary>
        ///     Starts the TCP listener
        /// </summary>
        void StartListen();

        /// <summary>
        ///     Stops the TCP listener
        /// </summary>
        void StopListen();

        /// <summary>
        ///     This event will be fired everytime a new widget is discovered. NB: StartDiscoveryService must be called to start
        ///     the discovery service
        /// </summary>
        event EventHandler<ContextFilterEventArgs> DiscoveryServiceEvent;

        /// <summary>
        ///     Starts the Widget Discovery Service
        /// </summary>
        void DiscoveryService();

        /// <summary>
        ///     Method for subscribing to a Situation.
        /// </summary>
        /// <param name="situationName">Name of situation to subscribe to</param>
        /// <param name="peer">The remote peer</param>
        void SubscribeSituation(string situationName, Peer peer);

        /// <summary>
        ///     Method for sending an entity
        /// </summary>
        /// <param name="entity">The entity to send</param>
        /// <param name="peer">The remote peer</param>
        void SendEntity(IEntity entity, Peer peer);

        /// <summary>
        ///     Method for sending an Situation state to client
        /// </summary>
        /// <param name="situation">The situation whoms state to send</param>
        /// <param name="peer">The remote peer</param>
        void SendSituationState(Situation situation, Peer peer);
    }
}