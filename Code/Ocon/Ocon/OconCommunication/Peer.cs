using System;

namespace Ocon.OconCommunication
{
    public class Peer : IOconPeer
    {
        public Peer(Guid id) { Id = id; }
        public Guid Id { get; private set; }
    }
}
