using Microsoft.Build.Framework;


namespace ContextawareFramework
{
    public interface IEntity
    {
        [Required()]
        System.Guid WidgetId { get; set; }
        [Required()]
        int Id { get; set; }

        string Name { get; set; }
        string Description { get; set; }
    }
}
