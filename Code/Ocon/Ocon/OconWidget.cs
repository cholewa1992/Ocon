using System;
using System.Collections.Generic;
using System.IO;
using Ocon.Entity;
using Ocon.Helpers;
using Ocon.OconCommunication;

namespace Ocon
{
    public class OconWidget
    {
        #region Properties

        private readonly OconComHelper _comHelper;
        private readonly Group _group;
        private readonly TextWriter _log;
        private readonly HashSet<IEntity> _trackedEntities = new HashSet<IEntity>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructs a new Widget
        /// </summary>
        /// <param name="comHelper">
        ///     The communication helper to use. A communication helper is needed for the widget to talk with
        ///     the context filter
        /// </param>
        /// <param name="log">Instance to write log information to</param>
        public OconWidget(OconComHelper comHelper, TextWriter log = null)
        {
            _comHelper = comHelper;
            _group = new Group(_comHelper);
            _log = log;
            StartDiscovery();
        }

        #endregion

        /// <summary>
        ///     This will start a discovery service that will find any avalible context filters on the local network
        /// </summary>
        private void StartDiscovery()
        {
            Logger.Write(_log, "Starting discovery");
            _comHelper.DiscoveryEvent += peer => _group.AddPeer(peer);
        }

        /// <summary>
        ///     This methode should be invoked when ever a tracked entity is updated. This will send the update to the context
        ///     filter
        /// </summary>
        /// <param name="entity">The entity that was updated</param>
        public void Notify(IEntity entity)
        {
            if (!_trackedEntities.Contains(entity))
            {
                entity.Id = Guid.NewGuid();
                entity.WidgetId = _comHelper.Address.Id;
                _trackedEntities.Add(entity);
            }
            _group.SendEntity(entity);
        }
    }
}
