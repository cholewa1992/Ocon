using System;

namespace Ocon.Messages
{
    public class SituationUnsubscriptionMessage : IOconMessage
    {
        public Guid SituationId { get; private set; }
        public MessageType Type { get; private set; }

        public SituationUnsubscriptionMessage(Guid situationId)
        {
            SituationId = situationId;
            Type = MessageType.Unsubscription;
        }
    }
}