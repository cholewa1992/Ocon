using System;
using Ocon.OconCommunication;

namespace Ocon.TcpCom
{
    class Peer : IOconPeer
    {
        public Peer(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; private set; }
    }
}
