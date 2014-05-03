using System;
using System.Collections.Generic;
using System.Net;
using ContextawareFramework.NetworkHelper;

namespace ContextawareFramework
{
    public class ContextCentral
    {

        private readonly ICommunicationHelper _comHelper;
        private readonly ContextFilter _contextFilter;


        public ContextCentral(ContextFilter contextFilter, ICommunicationHelper communicationHelper)
        {

            _contextFilter = contextFilter;
            _comHelper = communicationHelper;

        }



        public void Initialize()
        {

            // Set up
            _contextFilter.SituationStateChanged            +=  (sender, args) =>
                                                                _comHelper.SendSituationState(args.Situation, args.Subscriber);

            // Set up events
            _comHelper.IncommingEntityEvent                 +=  (sender, args) => 
                                                                _contextFilter.TrackEntity(args.Entity);

            _comHelper.IncommingSituationSubscribtionEvent  +=  (sender, args) => 
                                                                _contextFilter.Subscribe(args.Peer, args.SituationIdentifier);

            // Start listening for widgets and clients
            _comHelper.StartListen();

            // Multicast presence to widgets
            _comHelper.Broadcast();
        }
    }
}