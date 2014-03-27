using System;
using System.Collections.Generic;
using ContextawareFramework;
using NetworkHelper;
using Newtonsoft.Json;

namespace Widget
{
    public class Widget
    {
        #region Properties
        private readonly HashSet<IEntity> _trackedEntities = new HashSet<IEntity>();
        private readonly ICommunicationHelper _comHelper;
        private readonly Group _group;

        /// <summary>
        /// An unique identifier. Works like a MAC address
        /// </summary>
        public Guid WidgetId { private set; get; }
        #endregion
        #region Constructors
        /// <summary>
        /// Constructs a new Widget
        /// </summary>
        /// <param name="comHelper">The communication helper to use. A communication helper is needed for the widget to talk with the context filter</param>
        public Widget(ICommunicationHelper comHelper)
        {
            _comHelper = comHelper;
            _group = new Group(_comHelper);
            WidgetId = Guid.NewGuid();
        }

        /// <summary>
        /// Constructs a new Widget
        /// </summary>
        /// <param name="comHelper">The communication helper to use. A communication helper is needed for the widget to talk with the context filter</param>
        /// <param name="guid">The guid to use. The guid works like a MAC address</param>
        public Widget(ICommunicationHelper comHelper, Guid guid)
        {
            _comHelper = comHelper;
            _group = new Group(_comHelper);
            WidgetId = guid;
        }
        #endregion

        /// <summary>
        /// This will start a discovery service that will find any avalible context filters on the local network
        /// </summary>
        public void StartDiscovery()
        {
            Console.WriteLine("Starting discovery (" + WidgetId + ")");
            _comHelper.DiscoveryServiceEvent += (sender, args) => _group.AddPeer(args.Peer);
            _comHelper.DiscoveryService(WidgetId);
        }

        /// <summary>
        /// This methode should be invoked when ever a tracked entity is updated. This will send the update to the context filter
        /// </summary>
        /// <param name="entity">The entity that was updated</param>
        public void Notify(IEntity entity)
        {
            RegisterEntity(entity);
            var msg = JsonConvert.SerializeObject(entity, entity.GetType(),new JsonSerializerSettings{TypeNameHandling = TypeNameHandling.Objects});
            _group.Send(msg);
        }

        /// <summary>
        /// Registers the enetity and givs it an unique Id.
        /// </summary>
        /// <param name="entity"></param>
        private void RegisterEntity(IEntity entity)
        {
            if (!_trackedEntities.Contains(entity))
            {
                entity.Id = Guid.NewGuid();
                entity.WidgetId = WidgetId;
                _trackedEntities.Add(entity);
            }
        }
    }
}