using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextawareFramework
{
    class SituationEqualityCompare : IEqualityComparer<ISituation>
    {
        public bool Equals(ISituation x, ISituation y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(ISituation obj)
        {
            return obj.GetHashCode();
        }
    }
}
