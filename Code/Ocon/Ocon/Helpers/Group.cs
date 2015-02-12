using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Ocon.Entity;
using Ocon.Messages;
using Ocon.OconCommunication;
using Ocon.TcpCom;

namespace Ocon.Helpers
{
    internal class Group
    {
        #region Fields

        private readonly OconComHelper _comHelper;
        private readonly HashSet<IOconPeer> _observers = new HashSet<IOconPeer>(new PeerEquallityCompare());
        private readonly Dictionary<IOconPeer, int> _tries = new Dictionary<IOconPeer, int>();

        #endregion

        /// <summary>
        ///     Constructs a new Communicationn Group
        /// </summary>
        /// <param name="comHelper">The ComHelper to use</param>
        public Group(OconComHelper comHelper)
        {
            _comHelper = comHelper;
        }

        /// <summary>
        ///     Sends a entity to all peers in the group
        /// </summary>
        /// <param name="entity">The entity to sent</param>
        public void SendEntity(IEntity entity)
        {
            var toRemove = new List<IOconPeer>();
            foreach (IOconPeer observer in _observers)
            {
                try
                {
                    _comHelper.Send(new EntityMessage(entity), observer);
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
        public void AddPeer(IOconPeer peer)
        {
            lock (_observers)
            {
                if (!_observers.Contains(peer))
                {
                    Console.WriteLine("Adding peer to group: " + peer.Id);
                    _observers.Add(peer);
                    _tries.Add(peer, 0);
                }
            }
        }

        /// <summary>
        ///     Removes a peer from the group
        /// </summary>
        /// <param name="peer"></param>
        public void RemovePeer(IOconPeer peer)
        {
            lock (_observers)
            {
                Console.WriteLine("Removing peer from group: " + peer.Id);
                _observers.Remove(peer);
            }
        }
    }
}