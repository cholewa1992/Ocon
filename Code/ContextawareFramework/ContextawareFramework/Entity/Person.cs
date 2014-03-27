using System;
namespace ContextawareFramework
{
    public class Person: AbstractEntity
    {
        public override string WidgetName { get; set; }
        public override string Name { get; set; }
        public override string Description { get; set; }
        public bool Present { get; set; }
    }
}