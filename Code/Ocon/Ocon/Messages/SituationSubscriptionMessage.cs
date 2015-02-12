namespace Ocon.Messages
{
    public class SituationSubscriptionMessage : IOconMessage
    {
        public SituationSubscriptionMessage(string situationName)
        {
            Type = MessageType.Subscription;
            SituationName = situationName;
        }

        public MessageType Type { get; private set; }
        public string SituationName { get; private set; }
    }
}