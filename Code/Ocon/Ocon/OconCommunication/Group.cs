using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Ocon.Entity;

namespace Ocon.OconCommunication
{
    internal class Group
    {
        #region Fields

        private readonly IOconCom _comHelper;
        private readonly HashSet<Peer> _observers = new HashSet<Peer>(new PeerEquallityCompare());
        private readonly Dictionary<Peer, int> _tries = new Dictionary<Peer, int>();

        #endregion

        /// <summary>
        ///     Constructs a new Communicationn Group
        /// </summary>
        /// <param name="comHelper">The ComHelper to use</param>
        public Group(IOconCom comHelper)
        {
            _comHelper = comHelper;
        }

        /// <summary>
        ///     Sends a entity to all peers in the group
        /// </summary>
        /// <param name="entity">The entity to sent</param>
        public void SendEntity(IEntity entity)
        {
            var toRemove = new List<Peer>();
            foreach (Peer observer in _observers)
            {
                try
                {
                    _comHelper.SendEntity(entity, observer);
                    _tries[observer] = 0;
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                    _tries[observer]++;
                    if (_tries[observer] >= 3) toRemove.Add(observer);
                }
            }
            foreach (Peer peer in toRemove)
            {
                RemovePeer(peer);
            }
        }


        /// <summary>
        ///     Adds a peer to the group
        /// </summary>
        /// <param name="peer">The peer to add</param>
        public void AddPeer(Peer peer)
        {
            lock (_observers)
            {
                if (!_observers.Contains(peer))
                {
                    Console.WriteLine("Adding peer to group: " + peer.Guid);
                    _observers.Add(peer);
                    _tries.Add(peer, 0);
                }
            }
        }

        /// <summary>
        ///     Removes a peer from the group
        /// </summary>
        /// <param name="peer"></param>
        public void RemovePeer(Peer peer)
        {
            lock (_observers)
            {
                Console.WriteLine("Removing peer from group: " + peer.Guid);
                _observers.Remove(peer);
            }
        }
    }
}