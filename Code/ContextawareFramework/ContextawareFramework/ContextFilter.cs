using System;
using System.Collections.Generic;

namespace ContextawareFramework
{
    public class ContextFilter
    {
        public List<Person> entities = new List<Person>();
        public List<Context> Contexts = new List<Context>();


        public void AddContext(Context context)
        {
            Contexts.Add(context);
        }

        public void EntitiesUpdated()
        {
            for(int i = 0; i < entities.Count; i++)
            {
                var result = String.Format("{0} : {1} - {2}", entities[i].GetType(), i, entities[i].i);
                Console.WriteLine(result);
            }


            Console.WriteLine(TestContext(Contexts[0]));
            
        }

        public bool TestContext(Context context)
        {

            
                for (int i = 0; i < entities.Count; i++)
                {
                    bool predicate = context.EntityPredicate.Invoke(entities[i]);

                    if (!predicate)
                        return false;

                    if (i == entities.Count-1)
                    {
                        return true;
                    }

                }


            return false;
        }


        
    }
}