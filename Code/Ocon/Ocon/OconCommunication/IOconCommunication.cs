using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Ocon.OconCommunication
{
    interface IOconCommunication
    {
        void Send(string msg, Peer peer);
        void Listen();
        event EventHandler RecievedPackage;
        void Broadcast(string msg);

    }
}
