using System;
using Microsoft.Build.Framework;


namespace ContextawareFramework
{
    public interface IEntity
    {

        Guid WidgetId { get; }
        string WidgetName { get; set; }
        Guid Id { get; }
        string Name { get; set; }
        string Description { get; set; }


    }
}
