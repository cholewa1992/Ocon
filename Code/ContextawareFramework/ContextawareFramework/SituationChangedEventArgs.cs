using System;
using ContextawareFramework.NetworkHelper;

namespace ContextawareFramework
{
    public class SituationChangedEventArgs: EventArgs
    {
        public ISituation Situation { get; set; }
        public Peer Subscriber { get; set; }
    }
}