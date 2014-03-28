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

    public class IncommingSituationEventArgs : EventArgs
    {
        public ISituation Situation { set; get; }
        internal IncommingSituationEventArgs(ISituation situation)
        {
            Situation = situation;
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