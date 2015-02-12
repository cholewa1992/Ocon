using System;
using System.Collections.Generic;
using System.Linq;
using Ocon.Entity;
using Ocon.OconCommunication;

namespace Ocon
{
    public class Situation
    {
        private readonly Guid _id = Guid.NewGuid();
        private readonly HashSet<IOconPeer> _peers = new HashSet<IOconPeer>(new PeerEquallityCompare());

        public Situation(string name, Predicate<ICollection<IEntity>> situationPredicate)
        {
            Name = name;
            SituationPredicate = situationPredicate;
            
        }

        public string Name { get; set; }

        public Guid Id
        {
            get { return _id; }
        }

        public string Description { get; set; }

        public bool State { get; set; }


        public Predicate<ICollection<IEntity>> SituationPredicate { get; set; }


        public void AddSubscriber(IOconPeer peer)
        {
            _peers.Add(peer);
        }

        public void RemoveSubscriber(IOconPeer peer)
        {
            _peers.Remove(peer);
        }

        public List<IOconPeer> GetSubscribersList()
        {
            return _peers.ToList();
        }
    }
}