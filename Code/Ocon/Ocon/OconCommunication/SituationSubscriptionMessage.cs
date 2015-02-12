namespace Ocon.OconCommunication
{
    public class SituationSubscriptionMessage : IOconMessage
    {
        public SituationSubscriptionMessage(string situationId, string situationName)
        {
            Type = MessageType.Subscription;
            SituationId = situationId;
            SituationName = situationName;
        }

        public MessageType Type { get; private set; }
        public string SituationId { get; private set; }
        public string SituationName { get; private set; }
    }
}