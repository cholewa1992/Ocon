using System;
using System.IO;
using Ocon.Helper;
using Ocon.OconCommunication;

namespace Ocon
{
    public class OconClient
    {
        private readonly OconComHelper _comHelper;
        private readonly TextWriter _log;
        private readonly string[] _situations;


        /// <summary>
        ///     Constructs a new Client
        /// </summary>
        /// <param name="comHelper">The IOconCom implementation to use</param>
        /// <param name="log">Instance to write log messages to</param>
        /// <param name="situations">The situations to send to the context framework for tracking</param>
        public OconClient(OconComHelper comHelper, TextWriter log = null, params string[] situations)
        {
            //Setting the com helper
            _comHelper = comHelper;
            _situations = situations;
            _log = log;

            //Staring discovery
            SetupCommunication();
        }

        /// <summary>
        ///     This will start a discovery service that will find any avalible context filters on the local network
        /// </summary>
        private void SetupCommunication()
        {
            //Forwarding Situation event changes to client
            _comHelper += (sender, args) => SituationStateChangedEvent.Invoke(this, args);

            _comHelper.DiscoveryServiceEvent += (sender, args) =>
            {
                foreach (string situation in _situations)
                {
                    _comHelper.SubscribeSituation(situation, args.Peer);
                }
            };

            //Start listening
            _comHelper.StartListen();

            Logger.Write(_log, "Starting discovery (" + _comHelper.Me.Guid + ")");
            _comHelper.DiscoveryService();
        }

        /// <summary>
        ///     This event is fired when a subscribed situation's state has changed
        /// </summary>
        public event SituationChangedHandler SituationStateChangedEvent;

        public delegate void SituationChangedHandler(Situation situation);
    }
}