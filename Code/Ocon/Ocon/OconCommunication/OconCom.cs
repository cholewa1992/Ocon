using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ocon.Entity;
using Ocon.Helper;

namespace Ocon.OconCommunication
{
    class OconCom : IOconCom
    {

        private IOconCommunication _com;
        private ISerialize _serializer;


        public Peer Me { get; private set; }
        public void Broadcast(int frequency = 30)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var msg = _serializer.Serialize(new Handshake {Peer = Me});
                    _com.Broadcast(msg);
                    Thread.Sleep(frequency * 1000);
                }
            });
        }

        public void StopBroadcast()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<IncommingSituationSubscribtionEventArgs> IncommingSituationSubscribtionEvent;
        public event EventHandler<IncommingEntityEventArgs> IncommingEntityEvent;
        public event EventHandler<IncommingSituationChangedEventArgs> IncommingSituationChangedEvent;
        public void StartListen()
        {
            throw new NotImplementedException();
        }

        public void StopListen()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ContextFilterEventArgs> DiscoveryServiceEvent;
        public void DiscoveryService()
        {
            throw new NotImplementedException();
        }

        public void SubscribeSituation(string situationName, Peer peer)
        {
            throw new NotImplementedException();
        }

        public void SendEntity(IEntity entity, Peer peer)
        {
           Send(entity, peer);
        }

        public void SendSituationState(Situation situation, Peer peer)
        {
            Send(situation, peer);
        }

        private void Send<T>(T obj, Peer peer)
        {
            var msg = _serializer.Serialize(obj);
            _com.Send(msg, peer);
        }

        private void Parse(Message message)
        {
            if (message.Type == PackageType.Entity)
            {
                //If no one is subsribing, continue
                if (IncommingEntityEvent == null) return;

                //Getting entity from JSON
                var entity = (IEntity)_serializer.Deserialize(message.Body);

                //Fireing Entity event
                IncommingEntityEvent(message.Peer, new IncommingEntityEventArgs(entity));
            }
            else if (message.Type == PackageType.SituationUpdate)
            {
                //Taking entity and stores it as dynamic
                dynamic situationUpdate = _serializer.Deserialize(message.Body);

                //Fireing event
                var eventArgs = new IncommingSituationChangedEventArgs((Guid)situationUpdate.SituationId,
                    (bool)situationUpdate.State, (string)situationUpdate.SituationName);
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
    }
}
