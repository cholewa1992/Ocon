using System;
using System.Collections.Generic;
using ContextawareFramework.NetworkHelper;

namespace ContextawareFramework
{
    public interface ISituation
    {
        string Name { get; set; }
        Guid Id { get; }
        string Description { get; set; }
        bool State { get; set; }
        Predicate<ICollection<IEntity>> SituationPredicate { get; set; }

        void AddSubscriber(Peer peer);
        void RemoveSubscriber(Peer peer);
        ICollection<Peer> GetSubscribersList();
    }
}
