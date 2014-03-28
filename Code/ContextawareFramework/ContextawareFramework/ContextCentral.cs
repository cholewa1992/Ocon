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
        private readonly Group _group;
        private Dictionary<Guid,IPEndPoint> _clients = new Dictionary<Guid, IPEndPoint>(); 


        public ContextCentral(ContextFilter contextFilter, ICommunicationHelper communicationHelper)
        {
            _contextFilter = contextFilter;
            _comHelper = communicationHelper;
            _group = new Group(communicationHelper);
        }

        public void Initialize()
        {
            _comHelper.StartListen();
            _comHelper.Broadcast();
            _comHelper.IncommingClient += (sender, args) => Console.WriteLine("New Client: " + args.Guid);
            _comHelper.IncommingEntityEvent += (sender, args) => _contextFilter.TrackEntity(args.Entity);
            _comHelper.IncommingSituationEvent += (sender, args) => _contextFilter.AddSituation(args.Situation);
        }
    }
}