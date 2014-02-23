using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;

namespace ContextawareFramework
{
    public class ContextFilter
    {

        public List<IEntity> _entities = new List<IEntity>();
        private readonly List<IContext> _contexts = new List<IContext>();



        public ContextFilter(IContext context)
        {
            _contexts.Add(context);
        }
       

        public bool RemoveContext(IContext context)
        {
            return _contexts.Remove(context);
        }

        public void AddContext(IContext context)
        {
            _contexts.Add(context);
        }

        public void EntitiesUpdated()
        {
            for(int i = 0; i < _entities.Count; i++)
            {
                var result = String.Format("{0} : {1} - {2}", _entities[i].GetType(), i, _entities[i].Name);
                Console.WriteLine(result);
            }


            Console.WriteLine(TestContext(_contexts[0]));
            
        }

        public bool TestContext(IContext context)
        {

            
                for (int i = 0; i < _entities.Count; i++)
                {
                    bool predicate = context.ContextPredicate.Invoke(_entities);

                    if (!predicate)
                        return false;

                    if (i == _entities.Count-1)
                    {
                        return true;
                    }

                }


            return false;
        }


        
    }
}