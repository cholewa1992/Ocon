using System;

namespace ContextawareFramework.NetworkHelper
{
    public class IncommingEntityEventArgs : EventArgs
    {
        public IEntity Entity { set; get; }
        internal IncommingEntityEventArgs(IEntity entity)
        {
            Entity = entity;
        }
    }

    public class IncommingSituationSubscribtionEventArgs : EventArgs
    {
        public Peer Peer { get; set; }
        public string SituationIdentifier { set; get; }
        internal IncommingSituationSubscribtionEventArgs(Peer peer, string situationIdentifier)
        {
            Peer = peer;
            SituationIdentifier = situationIdentifier;
        }
    }

    public class IncommingSituationChangedEventArgs : EventArgs
    {
        public Guid Guid { set; get; }
        public bool State { set; get; }
        internal IncommingSituationChangedEventArgs(Guid guid, bool state)
        {
            Guid = guid;
            State = state;
        }
    }

    /// <summary>
    /// Custom EventArgs to use for notification about new Widgets
    /// </summary>
    public class ContextFilterEventArgs : EventArgs
    {
        public Peer Peer { get; set; }

        internal ContextFilterEventArgs(Peer peer)
        {
            Peer = peer;
        }
    }
}