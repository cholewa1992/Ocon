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
        private Dictionary<Guid,IPEndPoint> _clients = new Dictionary<Guid, IPEndPoint>(); 


        public ContextCentral(ContextFilter contextFilter, ICommunicationHelper communicationHelper)
        {
            _contextFilter = contextFilter;
            _comHelper = communicationHelper;
        }

        public void Initialize()
        {
            // Set up events
            _comHelper.IncommingClient          += (sender, args) => _contextFilter.AddClient(args.Guid, args.Ipep);
            _comHelper.IncommingEntityEvent     += (sender, args) => _contextFilter.TrackEntity(args.Entity);
            _comHelper.IncommingSituationEvent  += (sender, args) => _contextFilter.AddSituation(args.Situation);

            // Start listening for widgets and clients
            _comHelper.StartListen();

            // Multicast presence to widgets
            _comHelper.Broadcast();
        }

        public void 
    }
}