using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ocon.Entity;

namespace Ocon
{
    public class OconComHelper
    {
        private readonly IOconCom _com;


        public OconCom(IOconCom com)
        {
            _com = com;

            _com.PeerDiscoveredEvent += peer => DiscoveryEvent(peer);
            _com.RecievedMessageEvent += (msg, sender) =>
            {
                switch (msg.Type)
                {
                    case MessageType.Entity:
                        EntityEvent(((EntityMessage) msg).Entity);
                        break;
                    case MessageType.Situation:
                        SituationEvent(((SituationMessage) msg).Situation);
                        break;
                    case MessageType.Subscription:
                        SituationSubscribtionEvent();
                        break;

                }
            };
        }

        /// <summary>
        ///     For broadcasting discovery pacakge so that peers can be auto-discovered. This metode runs on a separate thread
        /// </summary>
        public void Broadcast(int frequency = 30)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    _com.Broadcast(new HandshakeMessage());
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
        public event SituationSubscribtionHandler SituationSubscribtionEvent;

        public delegate void SituationSubscribtionHandler();

        /// <summary>
        ///     This event will be fired whenever a new entity is avalible
        /// </summary>
        public event EntityHandler EntityEvent;

        public delegate void EntityHandler(IEntity e);


        /// <summary>
        ///     This event will be fired whenever a situations state changes
        /// </summary>
        public event SituationHandler SituationEvent;

        public delegate void SituationHandler(Situation s);

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

    }
}
