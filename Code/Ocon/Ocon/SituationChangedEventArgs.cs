using System;
using Ocon.OconCommunication;

namespace Ocon
{
    public class SituationChangedEventArgs : EventArgs
    {
        public Situation Situation { get; set; }
        public IOconPeer Subscriber { get; set; }
    }
}