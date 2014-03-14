using System;
using System.Collections.Generic;
using ContextawareFramework;
using NetworkHelper;
using Newtonsoft.Json;

namespace Widget
{
    public class Widget
    {
        public Guid WidgetId { private set; get; }

        private int _idCounter;
        private readonly HashSet<IEntity> _trackedEntities = new HashSet<IEntity>();
        private readonly Group _group = new Group();

        public Widget()
        {
            WidgetId = Guid.NewGuid();
            Initialize();
        }

        public Widget(Guid guid)
        {
            WidgetId = guid;
            Initialize();
        }

        private void Initialize()
        {
            Console.WriteLine("Starting widget (" + WidgetId + ")");
            TcpHelper.DiscoveryServiceEvent += (sender, args) => _group.AddObserver(args.Peer);
            TcpHelper.DiscoveryService(WidgetId);
        }

        private int GetNextEntityId()
        {
            return _idCounter++;
        }

        public void Notify(IEntity entity)
        {
            var msg = JsonConvert.SerializeObject(entity);
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