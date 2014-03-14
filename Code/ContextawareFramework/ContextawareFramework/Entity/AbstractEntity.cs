using System;

namespace ContextawareFramework
{
    public abstract class AbstractEntity: IEntity
    {

        private readonly Guid _widgetId = Guid.NewGuid();
        public Guid WidgetId
        {
            get { return _widgetId; }
        }

        public abstract string WidgetName { get; set; }


        private readonly Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return _id; }
        }

        public abstract string Name { get; set; }
        public abstract string Description { get; set; }


        public override string ToString()
        {
            return "Name:" + Name
                   + " from widget: " + WidgetName;
        }
    }
}