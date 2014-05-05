using System;
using System.Collections.Generic;

namespace ContextawareFramework.NetworkHelper
{
    public class Peer
    {
        public Guid Guid { get; set; }
    }

    public class PeerEquallityCompare : IEqualityComparer<Peer>
    {
        public bool Equals(Peer x, Peer y)
        {
            return x.Guid.Equals(y.Guid);
        }

        public int GetHashCode(Peer obj)
        {
            return obj.Guid.GetHashCode();
        }
    }
}