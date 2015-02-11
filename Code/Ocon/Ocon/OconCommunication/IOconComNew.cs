using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocon.OconCommunication
{
    public delegate void RecievedEventHandler(string msg, MessageType type, IOconPeer sender);
    public delegate void PeerDiscoveredHandler(IOconPeer peer);
    public interface IOconComNew
    {
        void Send(string msg, IOconPeer reciever);
        void Broadcast(string msg);
        event RecievedEventHandler RecievedMessageEvent;
        event PeerDiscoveredHandler PeerDiscoveredEvent;
    }

    public enum MessageType
    {
        Direct,
        Broadcast
    }

    public interface IOconPeer
    {
        Guid Id { get; set; }
    }

}
