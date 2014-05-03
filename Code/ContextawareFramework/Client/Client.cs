using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ContextawareFramework;
using ContextawareFramework.NetworkHelper;

namespace Client
{
    public class Client
    {
        private readonly ICommunicationHelper _comHelper;
        private readonly string[] _situations;

        /// <summary>
        /// Constructs a new Client
        /// </summary>
        /// <param name="comHelper">The ICommunicationHelper implementation to use</param>
        /// <param name="situations">The situations to send to the context framework for tracking</param>
        public Client(ICommunicationHelper comHelper, params string[] situations)
        {
            //Setting the com helper
            _comHelper = comHelper;
            _situations = situations;

            //Staring discovery
            SetupCommunication();
        }

        /// <summary>
        /// This will start a discovery service that will find any avalible context filters on the local network
        /// </summary>
        private void SetupCommunication()
        {
            //Forwarding Situation event changes to client
            _comHelper.IncommingSituationChangedEvent +=
                (sender, args) => Console.WriteLine("Cool");

            _comHelper.DiscoveryServiceEvent += (sender, args) =>
            {
                foreach (var situation in _situations)
                {
                    _comHelper.SubscribeSituation(situation, args.Peer);
                }
            };

            //Start listening
            _comHelper.StartListen();

            Console.WriteLine("Starting discovery (" + _comHelper.Me.Guid + ")");
            _comHelper.DiscoveryService();
        }

        public event EventHandler<SituationStateUpdateEventArgs> SituationStateChangedEvent;

    }

    public class SituationStateUpdateEventArgs : EventArgs
    {
        public ISituation Situation { get; set; }

        public SituationStateUpdateEventArgs(ISituation situation)
        {
            Situation = situation;
        }
    }
}
