using System;
using System.IO;
using System.Net;

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
        public Guid Guid { get; set; }
        public string SituationIdentifier { set; get; }
        internal IncommingSituationSubscribtionEventArgs(Guid guid, string situationIdentifier)
        {
            Guid = guid;
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

    public class IncommingClientEventArgs : EventArgs
    {
        public IPEndPoint Ipep { set; get; }
        public Guid Guid { set; get; }

        internal IncommingClientEventArgs(IPEndPoint ipep, Guid guid)
        {
            Ipep = ipep;
            Guid = guid;
        }
    }

    /// <summary>
    /// Custom EventArgs to use for notification about new Widgets
    /// </summary>
    public class ContextFilterEventArgs : EventArgs
    {
        public Peer Peer { get; set; }

        internal ContextFilterEventArgs(IPEndPoint ipep)
        {
            Peer = new Peer { IpEndPoint = ipep };
        }
    }
}