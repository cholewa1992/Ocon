using System;

namespace Ocon.Entity
{
    public abstract class AbstractEntity : IEntity
    {
        
        public override string ToString()
        {
            return "Name:" + Name + " from widget: " + WidgetName;
        }

        public string WidgetName { get; set; }
        public Guid Id { get; set; }
        public Guid WidgetId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}