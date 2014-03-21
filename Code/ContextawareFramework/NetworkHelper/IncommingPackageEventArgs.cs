using System;
using System.IO;

namespace NetworkHelper
{
    public class IncommingPackageEventArgs : EventArgs
    {
        public string Message { set; get; }
        internal IncommingPackageEventArgs(string msg)
        {
            Message = msg;
        }
    }

    public class IncommingStreamEventArgs : EventArgs
    {
        public Stream Stream { set; get; }
        internal IncommingStreamEventArgs(Stream stream)
        {
            Stream = stream;
        }
    }

    public class IncommingClientEventArgs : EventArgs
    {
        public Guid Guid { set; get; }
        public ClientType ClientType { get; set; }

        internal IncommingClientEventArgs(Guid guid, ClientType clientType)
        {
            ClientType = clientType;
            Guid = guid;
        }
    }
}