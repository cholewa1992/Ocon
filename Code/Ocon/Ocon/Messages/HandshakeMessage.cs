using System;
using Ocon.OconCommunication;

namespace Ocon.Messages
{
    public class HandshakeMessage : IOconMessage
    {
        public DeviceType DeviceType { get; private set; }
        public MessageType Type { get; private set; }

        public HandshakeMessage(DeviceType deviceType)
        {
            DeviceType = deviceType;
            Type = MessageType.Handshake;
        }

    }
}
