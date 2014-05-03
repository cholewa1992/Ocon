using System;
using System.Collections.Generic;
using System.Net;

namespace ContextawareFramework
{
    public class ContextFilter
    {

        private readonly ICollection<IEntity> _entities = new HashSet<IEntity>(new EntityEquallityCompare());
        //private readonly ICollection<ISituation> _subscriptions = new HashSet<ISituation>();
        private readonly Dictionary<ISituation, List<Guid>> _subscriptions = new Dictionary<ISituation, List<Guid>>(new SituationEqualityCompare()); 

        

        public event EventHandler<SituationChangedEventArgs> SituationStateChanged;
        public void FireSituationStateChanged(ISituation situation)
        {
            if(SituationStateChanged != null) SituationStateChanged.Invoke(this, new SituationChangedEventArgs(){Situation = situation});
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
            

            TestSituations();
        }

        /// <summary>
        /// Removes an ISituation instance from the collection of recognized situations
        /// </summary>
        /// <param name="situation">An ISituation instance</param>
        /// <returns></returns>
        public bool RemoveSituation(ISituation situation)
        {
            if(situation == null) throw new ArgumentNullException("Parsed situation can't be null");

            return _subscriptions.Remove(situation);
        }


        /// <summary>
        /// Adds an ISituation instance to the collection of recognized situations
        /// </summary>
        /// <param name="situation">An ISituation instance</param>
        /// <param name="situations">zero or more ISituation instances</param>
        public void AddSituation(ISituation situation, params ISituation[] situations)
        {
            if(situation == null) throw new ArgumentNullException("Parsed situation can't be null");


            if (!_subscriptions.ContainsKey(situation))
            {
                _subscriptions.Add(situation, new List<Guid>());
            }

            
            foreach (var s in situations)
            {
                if (!_subscriptions.ContainsKey(s))
                {
                    _subscriptions.Add(s, new List<Guid>());
                }
            }
        }


        public void Subscribe(Guid subscriber, string situationIdentifier)
        {
            if (subscriber == null) throw new ArgumentNullException("Parsed guid can't be null");
            if (string.IsNullOrEmpty(situationIdentifier)) throw new ArgumentNullException("Parsed situationIdentifier can't be null or empty");


            foreach (var subscription in _subscriptions)
            {
                if (subscription.Key.Name == situationIdentifier)
                {
                    subscription.Value.Add(subscriber);
                }
            }

            _subscriptions[situation].Add(subscriber);

        }



        public void TestSituations()
        {
            foreach (var situation in _subscriptions)
            {

                Console.WriteLine("Situation id: " + situation.Key.Id);

                foreach (var subscriber in situation.Value)
                {
                    Console.WriteLine(subscriber);
                }
                
            }
        }


       
        
    }
}