using System;
using Ocon.Entity;

namespace Ocon.OconCommunication
{
    public class IncommingEntityEventArgs : EventArgs
    {
        internal IncommingEntityEventArgs(IEntity entity)
        {
            Entity = entity;
        }

        public IEntity Entity { set; get; }
    }

    public class IncommingSituationSubscribtionEventArgs : EventArgs
    {
        internal IncommingSituationSubscribtionEventArgs(Peer peer, string situationIdentifier)
        {
            Peer = peer;
            SituationIdentifier = situationIdentifier;
        }

        public Peer Peer { get; set; }
        public string SituationIdentifier { set; get; }
    }

    public class IncommingSituationChangedEventArgs : EventArgs
    {
        internal IncommingSituationChangedEventArgs(Guid guid, bool state, string situationName)
        {
            Guid = guid;
            State = state;
            SituationName = situationName;
        }

        public Guid Guid { set; get; }
        public bool State { set; get; }
        public string SituationName { get; set; }
    }

    /// <summary>
    ///     Custom EventArgs to use for notification about new Widgets
    /// </summary>
    public class ContextFilterEventArgs : EventArgs
    {
        internal ContextFilterEventArgs(Peer peer)
        {
            Peer = peer;
        }

        public Peer Peer { get; set; }
    }
}