using System;
using System.Net;

namespace NetworkHelper
{
    /// <summary>
    /// Custom EventArgs to use for notification about new Widgets
    /// </summary>
    public class ContextFilterEventArgs : EventArgs
    {
        public Peer Peer { get; set; }

        internal ContextFilterEventArgs(IPEndPoint ipep)
        {
            Peer = new Peer { IpEndPoint = ipep };
        }
    }
}