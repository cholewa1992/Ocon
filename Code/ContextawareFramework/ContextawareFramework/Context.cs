using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ContextawareFramework
{
    
    public class Context: IContext
    {
        
        
        public Predicate<ICollection<IEntity>> ContextPredicate { get; set; }

    }
}