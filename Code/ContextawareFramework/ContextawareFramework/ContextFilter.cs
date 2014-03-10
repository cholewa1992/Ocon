using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Services;
using Newtonsoft.Json;

namespace ContextawareFramework
{
    public class ContextFilter
    {
        private readonly ICollection<IEntity> _entities = new HashSet<IEntity>(new CustomEquallityCompare());
        private readonly ICollection<IContext> _contexts = new List<IContext>();

        public class CustomEquallityCompare : IEqualityComparer<IEntity>
        {
            public bool Equals(IEntity x, IEntity y)
            {
                return x.WidgetId == y.WidgetId || x.Id == y.Id;
            }

            public int GetHashCode(IEntity obj)
            {
                return obj.WidgetId.GetHashCode() + obj.Id;
            }
        }

        public ContextFilter(IContext context)
        {
            _contexts.Add(context);
            NetworkHelper.IncommingTcpEvent += (sender, args) =>
            {
                try
                {
                    var person = JsonConvert.DeserializeObject<Person>(args.Message);
                    TrackEntity(person);
                }
                catch (Exception e)
                {

                }
            };
            NetworkHelper.StartTcpListen();
            NetworkHelper.Broadcast();
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