using System;
using Ocon.Messages;

namespace Ocon.OconCommunication
{
    public delegate void RecievedEventHandler(IOconMessage msg, IOconPeer sender);
    public interface IOconComClient : IDisposable
    {
        void Send(IOconMessage msg, IOconPeer reciever);
        void Broadcast(IOconMessage msg);
        event RecievedEventHandler RecievedMessageEvent;
        IOconPeer Address { get;}
    }
}