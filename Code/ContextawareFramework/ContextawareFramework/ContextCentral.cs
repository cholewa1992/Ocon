using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using NetworkHelper;
using Newtonsoft.Json;

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

            _comHelper.IncommingStreamEvent += (sender, args) => _contextFilter.AddSituation((ISituation) new BinaryFormatter().Deserialize(args.Stream));
            
            _comHelper.HandshakeEvent += (sender, args) => _clients.Add(args.Guid, args.Ipep);

            _comHelper.IncommingStringEvent += (sender, args) => _contextFilter.TrackEntity(null);

            _comHelper.Broadcast();
        }
    }
}