using System;
using System.Collections.Generic;
using System.Linq;
using ContextawareFramework;
using ContextawareFramework.NetworkHelper;

namespace Client
{
    public class Client
    {
        private readonly Guid _clientId = Guid.NewGuid();
        private readonly ICommunicationHelper _comHelper;
        private readonly ICollection<ISituation> _situations = new HashSet<ISituation>();

        /// <summary>
        /// Constructs a new Client
        /// </summary>
        /// <param name="comHelper">The ICommunicationHelper implementation to use</param>
        /// <param name="situations">The situations to send to the context framework for tracking</param>
        public Client(ICommunicationHelper comHelper, params string[] situations)
        {
            //Setting the com helper
            _comHelper = comHelper;

            //Adding situations
            foreach (var s in situations)
            {
                
            }

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
                (sender, args) =>
                    SituationStateChangedEvent(this, new SituationStateUpdateEventArgs(_situations.Single(t => t.SubscribersAddresse == args.Guid)));


            //Start listening
            _comHelper.StartListen();

            Console.WriteLine("Starting discovery (" + _clientId + ")");
            _comHelper.DiscoveryService(_clientId);
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
