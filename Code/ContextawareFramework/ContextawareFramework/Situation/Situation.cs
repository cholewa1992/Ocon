using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using ContextawareFramework.NetworkHelper;

namespace ContextawareFramework
{
    [Serializable()]
    public class Situation: ISituation
    {

        private readonly HashSet<Peer> _peers = new HashSet<Peer>(new PeerEquallityCompare());
        private readonly Guid _id = Guid.NewGuid();

        public string Name { get; set; }

        public Guid Id
        {
            get { return _id; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(value), "Description argument can't be null or empty");
                _description = value;
            }
        }

        public bool State { get; set; }


        private Predicate<ICollection<IEntity>> _situationPredicate;
        public Predicate<ICollection<IEntity>> SituationPredicate
        {
            get { return _situationPredicate; }
            set
            {
                _situationPredicate = value;
            }
        }


        public void AddSubscriber(Peer peer)
        {
            _peers.Add(peer);
        }

        public void RemoveSubscriber(Peer peer)
        {
            _peers.Remove(peer);
        }

        public ICollection<Peer> GetSubscribersList()
        {
            return _peers.ToList();
        }

        public ICollection<Guid> Subscribers { get; set; }

        public Guid SubscribersAddresse { get; set; }


        public Situation(Predicate<ICollection<IEntity>> situationPredicate)
        {
            SituationPredicate = situationPredicate;
        }
    }
}