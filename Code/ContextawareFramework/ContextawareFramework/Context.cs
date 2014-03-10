using System;
using System.Collections.Generic;

namespace ContextawareFramework
{
    
    public class Context: IContext
    {
        public Predicate<ICollection<IEntity>> ContextPredicate { get; set; }
    }
}