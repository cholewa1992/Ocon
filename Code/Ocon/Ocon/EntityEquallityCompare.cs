using System.Collections.Generic;
using Ocon.Entity;

namespace Ocon
{
    public class EntityEquallityCompare : IEqualityComparer<IEntity>
    {
        public bool Equals(IEntity x, IEntity y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(IEntity obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}