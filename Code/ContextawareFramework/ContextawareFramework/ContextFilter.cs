using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;

namespace ContextawareFramework
{
    public class ContextFilter
    {
        public ICollection<IEntity> _entities = new List<IEntity>();
        private readonly ICollection<IContext> _contexts = new List<IContext>();

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
            var i = 0;
            foreach (var entity in _entities)
            {
                var result = String.Format("{0} : {1} - {2}", entity.GetType(), i, entity.Name);
                Console.WriteLine(result);
                i++;
            }
            Console.WriteLine(TestContext(_contexts.First()));
        }

        public bool TestContext(IContext context)
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                bool predicate = context.ContextPredicate.Invoke(_entities);

                if (!predicate)
                    return false;

                if (i == _entities.Count - 1)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public static class ListExtensions
    {
        public static IEnumerable<T> GetAllWithType<T>(this ICollection collection) where T : class
        {
            return collection.OfType<T>();
        }
    }
}