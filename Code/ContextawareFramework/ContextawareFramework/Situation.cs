using System;
using System.Collections.Generic;

namespace ContextawareFramework
{
    
    public class Situation: ISituation
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public Predicate<ICollection<IEntity>> SituationPredicate { get; set; }
    }
}