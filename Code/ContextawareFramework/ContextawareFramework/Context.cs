using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ContextawareFramework
{
    
    public class Context: IContext
    {
        
        
        public Predicate<List<IEntity>> ContextPredicate { get; set; }

    }
}