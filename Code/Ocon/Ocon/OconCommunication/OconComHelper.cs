using System;
using System.Threading;
using System.Threading.Tasks;
using Ocon.Entity;
using Ocon.Messages;

namespace Ocon.OconCommunication
{
    public class OconComHelper : IDisposable
    {
        private readonly IOconComClient _com;

        public IOconPeer Address
        {
            get
            {
                return _com.Address;
            }
        }

        public OconComHelper(IOconComClient com)
        {
            _com = com;
            _com.RecievedMessageEvent += (msg, peer) =>
            {
                switch (msg.Type)
                {
                    case MessageType.Handshake:
                        if (DiscoveryEvent != null)
                            DiscoveryEvent(peer);
                        break;
                    case MessageType.Entity:
                        if (EntityEvent != null)
                            EntityEvent(((EntityMessage) msg).Entity);
                        break;
                    case MessageType.Situation:
                        if (SituationEvent != null)
                            SituationEvent(((SituationMessage) msg).Situation);
                        break;
                    case MessageType.Subscription:
                        if (SituationSubscribtionEvent != null)
                            SituationSubscribtionEvent(((SituationSubscriptionMessage) msg).Situation, peer);
                        break;
                    case MessageType.Unsubscription:
                        if (SituationUnsubscribtionEvent != null)
                            SituationUnsubscribtionEvent(((SituationUnsubscriptionMessage) msg).SituationId, peer);
                        break;
                }
            };
        }

        private bool _broadcasting;
        /// <summary>
        ///     For broadcasting discovery pacakge so that peers can be auto-discovered. This metode runs on a separate thread
        /// </summary>
        public void Broadcast(DeviceType deviceType, int frequency = 30)
        {
            if (_broadcasting) return;
            _broadcasting = true;
            
            Task.Run(() =>
            {
                while (_broadcasting)
                {
                    _com.Broadcast(new HandshakeMessage(deviceType));
                    Thread.Sleep(frequency*1000);
                }
            });

        }

        //public event EventHandler<IncommingClientEventArgs> IncommingClient;
        /// <summary>
        ///     This event will be fired whenever a new client is avalible
        /// </summary>
        /// <summary>
        ///     This event will be fired whenever a new situation is avalible
        /// </summary>
        public event SituationUnsubscribtionHandler SituationUnsubscribtionEvent;

        public delegate void SituationUnsubscribtionHandler(Guid situationId, IOconPeer peer);

        //public event EventHandler<IncommingClientEventArgs> IncommingClient;
        /// <summary>
        ///     This event will be fired whenever a new client is avalible
        /// </summary>
        /// <summary>
        ///     This event will be fired whenever a new situation is avalible
        /// </summary>
        public event SituationSubscribtionHandler SituationSubscribtionEvent;

        public delegate void SituationSubscribtionHandler(IOconSituation situation, IOconPeer peer);

        /// <summary>
        ///     This event will be fired whenever a new entity is avalible
        /// </summary>
        public event EntityHandler EntityEvent;

        public delegate void EntityHandler(IEntity e);


        /// <summary>
        ///     This event will be fired whenever a situations state changes
        /// </summary>
        public event SituationHandler SituationEvent;

        public delegate void SituationHandler(IOconSituation s);

        /// <summary>
        ///     This event will be fired everytime a new widget is discovered. NB: StartDiscoveryService must be called to start
        ///     the discovery service
        /// </summary>
        public event DiscoveryHandler DiscoveryEvent;

        public delegate void DiscoveryHandler(IOconPeer peer);

        public void Send(IOconMessage msg, IOconPeer peer)
        {
            _com.Send(msg, peer);
        }

        public void Dispose()
        {
            _com.Dispose();
        }
    }
}
