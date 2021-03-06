﻿using System.IO;
using Ocon.Helpers;
using Ocon.Messages;
using Ocon.OconCommunication;

namespace Ocon
{
    public class OconCentral
    {
        private readonly OconComHelper _comHelper;
        private readonly OconContextFilter _contextFilter;
        private readonly TextWriter _log;

        /// <summary>
        ///     Constructs the object given contextFilter and oconCom
        /// </summary>
        /// <param name="contextFilter"></param>
        /// <param name="oconCom"></param>
        public OconCentral(OconContextFilter contextFilter, OconComHelper oconCom,
            TextWriter log = null)
        {
            _contextFilter = contextFilter;
            _comHelper = oconCom;
            _log = log;
            Initialize();
        }

        /// <summary>
        ///     Registers events and starts communication
        /// </summary>
        private void Initialize()
        {
            // Set up
            _contextFilter.SituationStateChanged +=
                (situation, subscriber) => _comHelper.Send(new SituationMessage(situation), subscriber);

            // Set up events
            _comHelper.EntityEvent += entity =>
            {
                _contextFilter.TrackEntity(entity);
                //Logger.Write(_log, "Incoming entity event: " + entity.Name);
            };



            _comHelper.SituationSubscribtionEvent += (situation, peer) =>
            {
                _contextFilter.Subscribe(peer, situation);
                Logger.Write(_log,
                    "Incoming situation form:" + peer.Id);
            };

            _comHelper.SituationUnsubscribtionEvent += (id, peer) => _contextFilter.Unsubscribe(peer, id);
        }
    }
}
