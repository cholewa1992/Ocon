using System;
using System.IO;
using System.Net;

namespace NetworkHelper
{
    public class StringEventArgs : EventArgs
    {
        public string Message { set; get; }
        internal StringEventArgs(string msg)
        {
            Message = msg;
        }
    }

    public class StreamEventArgs : EventArgs
    {
        public Stream Stream { set; get; }
        internal StreamEventArgs(Stream stream)
        {
            Stream = stream;
        }
    }

    public class HandshakeEventArgs : EventArgs
    {
        public Guid Guid { set; get; }

        internal HandshakeEventArgs(Guid guid)
        {
            Guid = guid;
        }
    }

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