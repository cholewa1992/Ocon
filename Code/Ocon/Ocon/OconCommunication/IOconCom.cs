using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Ocon.OconCommunication
{
    public delegate void RecievedEventHandler(IOconMessage msg, IOconPeer sender);
    public delegate void PeerDiscoveredHandler(IOconPeer peer);
    public interface IOconCom
    {
        void Send(IOconMessage msg, IOconPeer reciever);
        void Broadcast(IOconMessage msg);
        event RecievedEventHandler RecievedMessageEvent;
        event PeerDiscoveredHandler PeerDiscoveredEvent;
        IOconPeer Address { get;}
    }
}
