using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextawareFramework
{
    public interface IContext
    {

        Predicate<ICollection<IEntity>> ContextPredicate { get; set; }

    }

}
