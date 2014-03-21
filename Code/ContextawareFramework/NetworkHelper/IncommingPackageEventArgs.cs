using System;

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
}