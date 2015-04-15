using System;

namespace Ocon.Entity
{
    public interface IEntity
    {
        //string WidgetName { get; set; }
        Guid Id { set; get; }
        Guid WidgetId { set; get; }
        //string Name { get; set; }
        //string Description { get; set; }
    }
}