using System;
using Microsoft.Build.Framework;


namespace ContextawareFramework
{
    public interface IEntity
    {
        Guid WidgetId { set; get; }
        string WidgetName { get; set; }
        Guid Id { set; get; }
        string Name { get; set; }
        string Description { get; set; }
    }
}
