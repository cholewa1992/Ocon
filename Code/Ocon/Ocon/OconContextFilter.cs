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
        private readonly Dictionary<string, Situation> _situations = new Dictionary<string, Situation>();
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
        /// <param name="situationName">Name of the situation to remove</param>
        /// <returns>Whether removal succeeded</returns>
        public bool RemoveSituation(string situationName)
        {
            if (string.IsNullOrEmpty(situationName)) throw new ArgumentNullException("Parsed situation can't be null");

            return _situations.Remove(situationName);
        }


        /// <summary>
        ///     Adds one or more situations to the collection
        /// </summary>
        /// <param name="situation">An ISituation instance</param>
        /// <param name="situations">zero or more ISituation instances</param>
        public void AddSituation(Situation situation, params Situation[] situations)
        {
            if (situation == null) throw new ArgumentNullException("Parsed situation can't be null");


            //Add the first situation
            _situations.Add(situation.Name, situation);


            //Add params if any
            if (situations == null) return;
            foreach (Situation s in situations)
            {
                _situations.Add(s.Name, s);
            }
        }


        /// <summary>
        ///     Subscribes an interesant to a situation given situation name
        /// </summary>
        /// <param name="subscriber">Guid of interesant</param>
        /// <param name="situationName">Situation name</param>
        public void Subscribe(IOconPeer subscriber, string situationName)
        {
            if (subscriber == null) throw new ArgumentNullException("Parsed guid can't be null");
            if (string.IsNullOrEmpty(situationName))
                throw new ArgumentNullException("Parsed situationIdentifier can't be null or empty");


            if (_situations.ContainsKey(situationName))
            {
                _situations[situationName].AddSubscriber(subscriber);
                FireSituationStateChanged(_situations[situationName], subscriber);
            }
        }


        /// <summary>
        ///     Tests situation predicates against entities
        /// </summary>
        private void TestSituations()
        {
            Console.Clear();

            foreach (var situation in _situations)
            {
                var currentState = (bool) situation.Value.SituationPredicate.Compile().DynamicInvoke(_entities);
                Console.WriteLine(currentState + " - " + _entities.Count);

                //Notify subscribers if there's a change in state
                if (currentState != situation.Value.State)
                {
                    situation.Value.State = currentState;

                    foreach (Peer subscriber in situation.Value.GetSubscribersList())
                    {
                        Console.WriteLine("ok");
                        FireSituationStateChanged(situation.Value, subscriber);
                    }
                }
            }
        }

        #endregion

        #region Situation Event

        /// <summary>
        ///     This event will be fired when a situations state has changed
        /// </summary>
        public event SituationChangedHandler SituationStateChanged;

        public delegate void SituationChangedHandler(Situation situation, IOconPeer subsriber);

        public void FireSituationStateChanged(Situation situation, IOconPeer subscriber)
        {
            if (SituationStateChanged != null)
                SituationStateChanged(situation, subscriber);
        }

        #endregion
    }
}
