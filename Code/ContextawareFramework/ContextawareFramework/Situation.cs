using System;
using System.Collections.Generic;

namespace ContextawareFramework
{
    
    public class Situation: ISituation
    {
        public Predicate<ICollection<IEntity>> SituationPredicate { get; set; }
    }
}