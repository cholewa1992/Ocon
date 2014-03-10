using System;

namespace ContextawareFramework
{
    public class Room : IEntity
    {
        public Guid WidgetId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}