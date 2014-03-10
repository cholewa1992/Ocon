using System.Collections.Generic;

namespace NetworkHelper
{
    public class PeerEquallityCompare : IEqualityComparer<Peer>
    {
        public bool Equals(Peer x, Peer y)
        {
            return x.IpEndPoint.Equals(y.IpEndPoint);
        }

        public int GetHashCode(Peer obj)
        {
            return obj.IpEndPoint.GetHashCode();
        }
    }
}