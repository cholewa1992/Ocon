using System;
using Ocon.OconCommunication;

namespace Ocon
{
    public class SituationChangedEventArgs : EventArgs
    {
        public Situation Situation { get; set; }
        public Peer Subscriber { get; set; }
    }
}