using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ContextawareFramework
{
    public class ContextFilter
    {
        private readonly ICollection<IEntity> _entities = new HashSet<IEntity>(new CustomEquallityCompare());
        private readonly ICollection<IContext> _contexts = new List<IContext>();

        public ContextFilter(IContext context)
        {
            _contexts.Add(context);
            NetworkHelper.TcpHelper.IncommingTcpEvent += (sender, args) =>
            {
                try
                {
                    var person = JsonConvert.DeserializeObject<Person>(args.Message);
                    TrackEntity(person);
                }
                catch
                {
                }
            };

            NetworkHelper.TcpHelper.StartTcpListen();
            NetworkHelper.TcpHelper.Broadcast();
        }

        private void TrackEntity(IEntity entity)
        {
            if (_entities.Contains(entity))
            {
                _entities.Remove(entity);
            }
            _entities.Add(entity);
            EntitiesUpdated();

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
            foreach (var context in _contexts)
            {
                Console.WriteLine(TestContext(context));
            }
        }

        public bool TestContext(IContext context)
        {
            return context.ContextPredicate.Invoke(_entities);
        }
    }
}