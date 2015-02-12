using System.Collections.Generic;

namespace Ocon.OconCommunication
{
    public class PeerEquallityCompare : IEqualityComparer<IOconPeer>
    {
        public bool Equals(IOconPeer x, IOconPeer y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(IOconPeer obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}