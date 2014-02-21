using System;
using System.Collections.Generic;

namespace ContextawareFramework
{
    public class Context
    {
        
        public Action ContextAction;
        public Predicate<Person> EntityPredicate;

        public void FireContext()
        {
            ContextAction.Invoke();
        }
    }
}