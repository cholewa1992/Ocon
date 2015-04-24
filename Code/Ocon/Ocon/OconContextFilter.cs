using System;
using System.Collections.Generic;
using System.IO;
using Ocon.Entity;
using Ocon.OconCommunication;
using Ocon.TcpCom;

namespace Ocon
{
    public class OconContextFilter
    {
        #region Fields

        private readonly ICollection<IEntity> _entities = new HashSet<IEntity>(new EntityEquallityCompare());
        private readonly Dictionary<Guid, IOconSituation> _situations = new Dictionary<Guid, IOconSituation>();
        private readonly TextWriter _log;

        #endregion


        public OconContextFilter(TextWriter log = null)
        {
            _log = log;
        }


        #region Entity handling

        /// <summary>
        ///     Add an IEntity instance to the collection beeing checked for situations
        /// </summary>
        /// <param name="entity"></param>
        public void TrackEntity(IEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("Parsed entity can't be null");

            entity.LastUpdate = DateTime.UtcNow;

            if (_entities.Contains(entity))
            {
                _entities.Remove(entity);
            }
            _entities.Add(entity);


            TestSituations();
        }

        #endregion

        #region Situation handling

        /// <summary>
        ///     Removes an ISituation given name
        /// </summary>
        /// <param name="situationId">Id of the situation to remove</param>
        /// <returns>Whether removal succeeded</returns>
        public bool RemoveSituation(Guid situationId)
        {
            if (situationId == null) throw new ArgumentNullException("Parsed situation can't be null");

            return _situations.Remove(situationId);
        }

        /// <summary>
        ///     Subscribes an interesant to a situation given situation name
        /// </summary>
        /// <param name="subscriber">Guid of interesant</param>
        /// <param name="situation">Situation</param>
        public void Subscribe(IOconPeer subscriber, IOconSituation situation)
        {
            if (subscriber == null) throw new ArgumentNullException("subscriber");
            if (situation == null) throw new ArgumentNullException("situation");


            if (!_situations.ContainsKey(situation.Id)) _situations.Add(situation.Id, situation);
            _situations[situation.Id].AddSubscriber(subscriber);
            TestSituation(situation);
        }

        public void Unsubscribe(IOconPeer subscriber, Guid id)
        {
            if(subscriber == null) throw new ArgumentException("subscriber");
            if(id == null) throw new ArgumentException("id");

            if (_situations.ContainsKey(id)) _situations.Remove(id);
        }


        /// <summary>
        ///     Tests situation predicates against entities
        /// </summary>
        private void TestSituations()
        {
            foreach (var situation in _situations.Values)
            {
              TestSituation(situation); 
            }
        }

        private void TestSituation(IOconSituation situation)
        {
            //Notify subscribers if there's a change in state
            if (situation.Evaluate(_entities))
            {
                foreach (var subscriber in situation.GetSubscribersList())
                {
                    FireSituationStateChanged(situation, subscriber);
                }
            }
        }

        #endregion

        #region Situation Event

        /// <summary>
        ///     This event will be fired when a situations state has changed
        /// </summary>
        public event SituationChangedHandler SituationStateChanged;

        public delegate void SituationChangedHandler(IOconSituation situation, IOconPeer subsriber);

        public void FireSituationStateChanged(IOconSituation situation, IOconPeer subscriber)
        {
            if (SituationStateChanged != null)
                SituationStateChanged(situation, subscriber);
        }

        #endregion
    }
}
