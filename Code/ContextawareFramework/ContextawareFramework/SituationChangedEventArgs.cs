using System;

namespace ContextawareFramework
{
    public class SituationChangedEventArgs: EventArgs
    {
        public ISituation Situation { get; set; }
    }
}