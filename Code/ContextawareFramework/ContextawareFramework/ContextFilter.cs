using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ContextawareFramework
{
    public class ContextFilter
    {
        private readonly ICollection<IEntity> _entities = new HashSet<IEntity>(new CustomEquallityCompare());
        private readonly ICollection<ISituation> _situations = new List<ISituation>();

        
        public event EventHandler EntitiesChangedEvent;
        public void FireEntitiesChangedEvent()
        {
            EntitiesChangedEvent.Invoke(this, EventArgs.Empty);
        }
      
        /// <summary>
        /// Add an IEntity instance to the collection beeing checked for situations
        /// </summary>
        /// <param name="entity"></param>
        public void TrackEntity(IEntity entity)
        {
            if(entity == null) throw new ArgumentNullException("Parsed entity can't be null");

            if (_entities.Contains(entity))
            {
                _entities.Remove(entity);
            }
            _entities.Add(entity);
            
            FireEntitiesChangedEvent();
        }

        /// <summary>
        /// Removes an ISituation instance from the collection of recognized situations
        /// </summary>
        /// <param name="situation">An ISituation instance</param>
        /// <returns></returns>
        public bool RemoveSituation(ISituation situation)
        {
            if(situation == null) throw new ArgumentNullException("Parsed situation can't be null");

            return _situations.Remove(situation);
        }


        /// <summary>
        /// Adds an ISituation instance to the collection of recognized situations
        /// </summary>
        /// <param name="situation">An ISituation instance</param>
        /// <param name="situations">zero or more ISituation instances</param>
        public void AddSituation(ISituation situation, params ISituation[] situations)
        {
            if(situation == null) throw new ArgumentNullException("Parsed situation can't be null");

            _situations.Add(situation);

            foreach (var s in situations)
            {
                _situations.Add(s);
            }
        }

        
        
    }
}