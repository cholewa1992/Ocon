using System.IO;
using Ocon.Helpers;
using Ocon.Messages;
using Ocon.OconCommunication;

namespace Ocon
{
    public class OconClient
    {
        private readonly OconComHelper _comHelper;
        private readonly TextWriter _log;
        private readonly IOconSituation[] _situations;


        /// <summary>
        ///     Constructs a new Client
        /// </summary>
        /// <param name="comHelper">The IOconCom implementation to use</param>
        /// <param name="log">Instance to write log messages to</param>
        /// <param name="situations">The situations to send to the context framework for tracking</param>
        public OconClient(OconComHelper comHelper, TextWriter log = null, params IOconSituation[] situations)
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
            _comHelper.SituationEvent += situation => SituationStateChangedEvent(situation);

            _comHelper.DiscoveryEvent += (peer) =>
            {
                foreach (IOconSituation situation in _situations)
                {
                    _comHelper.Send(new SituationSubscriptionMessage(situation), peer);
                }
            };

            //Start listening
            //_comHelper.Listen();

            Logger.Write(_log, "Starting discovery");
            //_comHelper.DiscoveryService();
        }

        /// <summary>
        ///     This event is fired when a subscribed situation's state has changed
        /// </summary>
        public event SituationChangedHandler SituationStateChangedEvent;

        public delegate void SituationChangedHandler(IOconSituation situation);
    }
}