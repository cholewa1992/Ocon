using System.Runtime.InteropServices;

namespace Ocon.Messages
{
    public class SituationMessage : IOconMessage
    {
        public IOconSituation Situation { get; set; }
        public MessageType Type { get; private set; }

        public SituationMessage(IOconSituation situation)
        {
            Situation = situation;
            Type = MessageType.Situation;
        }


    }
}