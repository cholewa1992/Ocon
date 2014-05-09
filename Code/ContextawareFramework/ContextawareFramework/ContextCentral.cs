using System;
using System.IO;
using ContextawareFramework.Helper;
using ContextawareFramework.NetworkHelper;

namespace ContextawareFramework
{
    public class ContextCentral
    {
        private readonly ICommunicationHelper _comHelper;
        private readonly ContextFilter _contextFilter;
        private readonly TextWriter _log;

        /// <summary>
        /// Constructs the object given contextFilter and communicationHelper
        /// </summary>
        /// <param name="contextFilter"></param>
        /// <param name="communicationHelper"></param>
        public ContextCentral(ContextFilter contextFilter, ICommunicationHelper communicationHelper, TextWriter log = null)
        {
            _contextFilter = contextFilter;
            _comHelper = communicationHelper;
            _log = log;
        }

        /// <summary>
        /// Registers events and starts communication
        /// </summary>

        public void Initialize()
        {
            // Set up
            _contextFilter.SituationStateChanged += (sender, args) => _comHelper.SendSituationState(args.Situation, args.Subscriber);

            // Set up events
            _comHelper.IncommingEntityEvent += (sender, args) =>
            {
                _contextFilter.TrackEntity(args.Entity);
                Logger.Write(_log, "Incoming entity event: " + args.Entity.Name);
            };
    
            _comHelper.IncommingSituationSubscribtionEvent += (sender, args) =>
            {
                _contextFilter.Subscribe(args.Peer, args.SituationIdentifier);
                Logger.Write(_log, "Incoming situation subscribtion on: " + args.SituationIdentifier + " form:" + args.Peer.Guid);
            };
    

            // Start listening for widgets and clients
            _comHelper.StartListen();

            // Multicast presence to widgets
            _comHelper.Broadcast();
        }
    }
}