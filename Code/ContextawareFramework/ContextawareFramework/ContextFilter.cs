using System;
using System.Collections.Generic;
using System.Net;

namespace ContextawareFramework
{
    public class ContextFilter
    {

        private readonly ICollection<IEntity> _entities = new HashSet<IEntity>(new CustomEquallityCompare());
        private readonly ICollection<ISituation> _situations = new HashSet<ISituation>();
        private readonly Dictionary<Guid, IPEndPoint>  _clients = new Dictionary<Guid, IPEndPoint>();

        

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



        public void TestSituations()
        {
            foreach (var situation in _situations)
            {
                Console.WriteLine(situation.SubscribersAddresse);
                if (situation.SituationPredicate == null) continue;
                Console.WriteLine("Ok");


                bool currentState = situation.SituationPredicate(_entities);

                if (situation.State != currentState)
                {
                    situation.State = currentState;
                    FireSituationStateChanged(situation);
                }

            }
        }


        public void AddClient(Guid guid, IPEndPoint ipEndPoint)
        {
            _clients.Add(guid, ipEndPoint);
        }

        public IPEndPoint GetClientEndPoint(Guid guid)
        {
            return _clients[guid];
        }
        
    }
}