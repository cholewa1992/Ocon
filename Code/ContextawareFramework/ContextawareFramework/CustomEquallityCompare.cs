using System.Collections.Generic;

namespace ContextawareFramework
{
    public class CustomEquallityCompare : IEqualityComparer<IEntity>
    {
        public bool Equals(IEntity x, IEntity y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(IEntity obj)
        {
            return obj.WidgetId.GetHashCode() * obj.Id.GetHashCode();
        }
    }
}