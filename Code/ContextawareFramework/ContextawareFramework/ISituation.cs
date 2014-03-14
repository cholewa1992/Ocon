using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextawareFramework
{
    public interface ISituation
    {

        string Id { get; set; }
        string Description { get; set; }
        Predicate<ICollection<IEntity>> SituationPredicate { get; set; }

    }

}
