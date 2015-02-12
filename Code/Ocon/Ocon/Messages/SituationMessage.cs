namespace Ocon.Messages
{
    public class SituationMessage : IOconMessage
    {
        public SituationMessage(Situation situation)
        {
            Situation = situation;
            Type = MessageType.Situation;;
        }

        public MessageType Type { get; private set; }
        public Situation Situation { get; private set; }
    }
}