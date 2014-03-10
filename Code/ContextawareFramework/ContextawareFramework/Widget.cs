using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace ContextawareFramework
{
    public class Widget
    {
        public Guid WidgetId { private set; get; }

        private int _idCounter = 0;
        private readonly HashSet<IEntity> _trackedEntities = new HashSet<IEntity>();
        private readonly HashSet<IPEndPoint> _observers = new HashSet<IPEndPoint>();


        public Widget()
        {
            WidgetId = Guid.NewGuid();
            NetworkHelper.DiscoveryService += (sender, args) =>
            {
                lock (_observers)
                {
                    _observers.Add(args.IpEndPoint);
                }
            };
            NetworkHelper.DiscoverContextFilter(WidgetId);
        }


        public Widget(Guid guid)
        {
            WidgetId = guid;
            NetworkHelper.DiscoveryService += (sender, args) =>
            {
                lock (_observers)
                {
                    Console.WriteLine("Got CF");
                    _observers.Add(args.IpEndPoint);
                }
            };
            NetworkHelper.DiscoverContextFilter(WidgetId);
        }

        private int GetNextEntityId()
        {
            return _idCounter++;
        }

        public void Notify(IEntity entity)
        {
            var msg = JsonConvert.SerializeObject(entity);

            foreach (var peer in _observers)
            {
                Console.WriteLine("Notifying: " + peer);
                NetworkHelper.SendTcpPackage(msg, peer);
            }
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
                _trackedEntities.Add(entity);
            }
        }
    }
}