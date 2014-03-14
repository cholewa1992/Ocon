using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ContextawareFramework
{
    public class ContextFilter
    {
        private readonly ICollection<IEntity> _entities = new HashSet<IEntity>(new CustomEquallityCompare());
        private readonly ICollection<ISituation> _situations = new List<ISituation>();


        /// <summary>
        /// 
        /// </summary>
        public ContextFilter()
        {
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

        /// <summary>
        /// Add an IEntity instance to the collection beeing checked for situations
        /// </summary>
        /// <param name="entity"></param>
        public void TrackEntity(IEntity entity)
        {
            if (_entities.Contains(entity))
            {
                _entities.Remove(entity);
            }
            _entities.Add(entity);
            EntitiesUpdated();
        }

        /// <summary>
        /// Removes an ISituation instance from the collection of recognized situations
        /// </summary>
        /// <param name="situation">An ISituation instance</param>
        /// <returns></returns>
        public bool RemoveSituation(ISituation situation)
        {
            return _situations.Remove(situation);
        }

        /// <summary>
        /// Adds an ISituation instance to the collection of recognized situations
        /// </summary>
        /// <param name="situation"></param>
        public void AddSituation(ISituation situation)
        {
            _situations.Add(situation);
        }


        public void EntitiesUpdated()
        {
            foreach (var context in _situations)
            {
                Console.WriteLine(TestContext(context));
            }
        }

        /// <summary>
        /// Test
        /// </summary>
        /// <param name="situation"></param>
        /// <returns></returns>
        public bool TestContext(ISituation situation)
        {
            return situation.SituationPredicate.Invoke(_entities);
        }
    }
}