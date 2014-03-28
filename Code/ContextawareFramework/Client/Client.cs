using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ContextawareFramework;
using NetworkHelper;

namespace Client
{
    public class ClientImp
    {
        private readonly Guid _clientId = Guid.NewGuid();
        private readonly ICommunicationHelper _comHelper;
        private readonly Group _group;

        public ClientImp(ICommunicationHelper comHelper)
        {
            _comHelper = comHelper;
            _group = new Group(comHelper);

        }

        /// <summary>
        /// This will start a discovery service that will find any avalible context filters on the local network
        /// </summary>
        public void StartDiscovery()
        {
            Console.WriteLine("Starting discovery (" + _clientId + ")");
            _comHelper.DiscoveryServiceEvent += (sender, args) => _group.AddPeer(args.Peer);
            _comHelper.DiscoveryService(_clientId, true);
        }

        public event EventHandler ContextEvent;

        public void RegisterSituation(ISituation situation)
        {
            var stream = new MemoryStream();
            new BinaryFormatter().Serialize(stream, situation);
            _group.Send(stream);
            
        }
    }
}
