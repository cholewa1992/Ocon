using System;

namespace ContextawareFramework
{
    public abstract class AbstractEntity: IEntity
    {

        private Guid _widgetId = Guid.NewGuid();
        public Guid WidgetId
        {
            get { return _widgetId; }
            set { _widgetId = value; }
        }

        public abstract string WidgetName { get; set; }


        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
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