using System;
namespace ContextawareFramework
{
    public class Person: IEntity
    {
        public int i;
        public Guid WidgetId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public override int GetHashCode()
        {
            return WidgetId.GetHashCode() + Id;
        }
    }
}