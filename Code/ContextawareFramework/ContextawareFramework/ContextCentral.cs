using System;
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



        public ContextCentral(ContextFilter contextFilter, ICommunicationHelper communicationHelper)
        {
            _contextFilter = contextFilter;
            _comHelper = communicationHelper;
            _group = new Group(communicationHelper);
        }

        

        public void Initialize()
        {
            _comHelper.StartListen(_comHelper.WidgetPort);
            _comHelper.StartListenForClient(_comHelper.ClientPort);
            _comHelper.IncommingClientEvent += (sender, args) => Console.WriteLine("Found: " + args.Guid);
            _comHelper.IncommingStreamEvent += (sender, args) => _contextFilter.AddSituation((ISituation) new BinaryFormatter().Deserialize(args.Stream));
            _comHelper.Broadcast();
        }
    }
}