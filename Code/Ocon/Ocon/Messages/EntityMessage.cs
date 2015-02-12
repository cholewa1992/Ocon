using Ocon.Entity;

namespace Ocon.Messages
{
    public class EntityMessage : IOconMessage
    {
        public EntityMessage(IEntity entity)
        {
            Type = MessageType.Entity;
            Entity = entity;
        }

        public MessageType Type { get; private set; }
        public IEntity Entity { get; private set; }
    }
}