using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocon.OconCommunication
{
    class HandshakeMessage : IOconMessage
    {
        public HandshakeMessage()
        {
            Type = MessageType.Handshake;
        }
        public MessageType Type { get; private set; }
    }
}
