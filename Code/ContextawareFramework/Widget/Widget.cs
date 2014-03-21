using System;
using System.Collections.Generic;
using ContextawareFramework;
using NetworkHelper;
using Newtonsoft.Json;

namespace Widget
{
    public class Widget
    {

        private readonly HashSet<IEntity> _trackedEntities = new HashSet<IEntity>();
        private readonly ICommunicationHelper _comHelper;
        private readonly Group _group;
        public Guid WidgetId { private set; get; }
        
        

        public Widget(ICommunicationHelper comHelper)
        {
            _comHelper = comHelper;
            _group = new Group(_comHelper);
            WidgetId = Guid.NewGuid();
            Initialize();
        }

        public Widget(ICommunicationHelper comHelper, Guid guid)
        {
            _comHelper = comHelper;
            _group = new Group(_comHelper);
            WidgetId = guid;
            Initialize();
        }

        private void Initialize()
        {

            Console.WriteLine("Starting widget (" + WidgetId + ")");
            _comHelper.DiscoveryServiceEvent += (sender, args) => _group.AddObserver(args.Peer);
            _comHelper.DiscoveryService(WidgetId, TcpHelper.StandardMulticastAddress);
        }

        private Guid GetNextEntityId()
        {
            return Guid.NewGuid();
        }

        public void Notify(IEntity entity)
        {
            var msg = JsonConvert.SerializeObject(entity);
#if DEBUG
            Console.WriteLine(msg);
#endif
            _group.Send(msg);
        }

        public void TrackEntity(IEntity entity)
        {
            if (!_trackedEntities.Contains(entity))
            {
                entity.Id = GetNextEntityId();
                entity.WidgetId = WidgetId;
                _trackedEntities.Add(entity);
            }
            else
            {
                _trackedEntities.Remove(entity);
                _trackedEntities.Add(entity);
            }
        }
    }
}