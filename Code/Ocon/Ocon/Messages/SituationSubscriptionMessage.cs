using System;
using System.Linq.Expressions;
using System.Runtime.Remoting;

namespace Ocon.Messages
{
    public class SituationSubscriptionMessage : IOconMessage
    {
        public IOconSituation Situation { get; private set; }
        public MessageType Type { get; private set; }

        public SituationSubscriptionMessage(IOconSituation situation)
        {
            Situation = situation;
            Type = MessageType.Subscription;
        }
    }
}