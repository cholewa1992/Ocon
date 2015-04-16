using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Ocon.Helpers;
using Ocon.Messages;
using Ocon.OconCommunication;

namespace Ocon
{
    public class OconClient
    {
        private readonly OconComHelper _comHelper;
        private readonly List<IOconSituation> _situations;
        private IOconPeer _peer;

        public IReadOnlyCollection<IOconSituation> Situations
        {
            get
            {
                return _situations.AsReadOnly();
            }
        }

        /// <summary>
        ///     Constructs a new Client
        /// </summary>
        /// <param name="comHelper">The IOconCom implementation to use</param>
        /// <param name="server">The central to connect to</param>
        public OconClient(OconComHelper comHelper, IOconPeer peer = null)
        {
            //Setting the com helper
            _comHelper = comHelper;
            _situations = new List<IOconSituation>();

            //Forwarding Situation event changes to client
            _comHelper.SituationEvent += situation =>
            {
                if (_situations.Any(s => s.Id == situation.Id))
                    SituationStateChangedEvent(situation);
            };

            _peer = peer;

            //Staring discovery
            if(peer == null) SetupCommunication();
        }

        /// <summary>
        ///     This will start a discovery service that will find any avalible context filters on the local network
        /// </summary>
        private void SetupCommunication()
        {
            _comHelper.DiscoveryEvent += (peer) =>
            {
                if(_peer != peer)
                    foreach (IOconSituation situation in _situations)
                    {
                        _comHelper.Send(new SituationSubscriptionMessage(situation), peer);
                    }
                    _peer = peer;
            };
        }

        public void Subscribe(IOconSituation situation)
        {
            _situations.Add(situation);
            if(_peer != null)
                _comHelper.Send(new SituationSubscriptionMessage(situation), _peer);
        }

        public void Unsubscribe(IOconSituation situation)
        {   
            if(_peer != null)
                _comHelper.Send(new SituationUnsubscriptionMessage(situation.Id), _peer);
            _situations.Remove(situation);
        }

        public void UnsubscribeAll()
        {
            foreach (var situation in _situations.ToList())
            {
                Unsubscribe(situation);
            }
        }

        /// <summary>
        ///     This event is fired when a subscribed situation's state has changed
        /// </summary>
        public event SituationChangedHandler SituationStateChangedEvent;

        public delegate void SituationChangedHandler(IOconSituation situation);
    }
}