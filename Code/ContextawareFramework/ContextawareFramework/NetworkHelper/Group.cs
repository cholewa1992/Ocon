using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ContextawareFramework.NetworkHelper
{
    public class Group
    {
        #region Fields
        private readonly ICommunicationHelper _comHelper;
        private readonly HashSet<Peer> _observers = new HashSet<Peer>(new PeerEquallityCompare());
        private readonly Dictionary<Peer, int> _tries = new Dictionary<Peer, int>();
        #endregion

        /// <summary>
        /// Constructs a new Communicationn Group
        /// </summary>
        /// <param name="comHelper">The ComHelper to use</param>
        public Group(ICommunicationHelper comHelper)
        {
            _comHelper = comHelper;
        }

        /// <summary>
        /// Sends a entity to all peers in the group
        /// </summary>
        /// <param name="entity">The entity to sent</param>
        public void SendEntity(IEntity entity)
        {
            var toRemove = new List<Peer>();
            foreach (var observer in _observers)
            {
                try
                {
                    _comHelper.SendEntity(entity, observer.IpEndPoint);
                    _tries[observer] = 0;

                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                    _tries[observer]++;
                    if (_tries[observer] >= 3) toRemove.Add(observer);
                }
            }
            foreach (var peer in toRemove)
            {
                RemovePeer(peer);
            }
        }


        /// <summary>
        /// Adds a peer to the group
        /// </summary>
        /// <param name="peer">The peer to add</param>
        public void AddPeer(Peer peer)
        {
            lock (_observers)
            {
                if (!_observers.Contains(peer))
                {
                    Console.WriteLine("Adding peer to group: " + peer.IpEndPoint);
                    _observers.Add(peer);
                    _tries.Add(peer, 0);

                }
            }
        }

        /// <summary>
        /// Removes a peer from the group
        /// </summary>
        /// <param name="peer"></param>
        public void RemovePeer(Peer peer)
        {
            lock (_observers)
            {
                Console.WriteLine("Removing peer from group: " + peer.IpEndPoint);
                _observers.Remove(peer);
            }
        }

        /// <summary>
        /// Helper class to print who's observing
        /// </summary>
        public void PrintObservers()
        {
            Console.Clear();
            Console.WriteLine("------------- Observers -------------");
            foreach (var observer in _observers)
            {
                Console.WriteLine(observer.IpEndPoint);
            }
            Console.WriteLine("-------------------------------------");
        }
    }

    #region Helperclasses
    public class Peer
    {
        public IPEndPoint IpEndPoint { get; set; }
    }

    public class PeerEquallityCompare : IEqualityComparer<Peer>
    {
        public bool Equals(Peer x, Peer y)
        {
            return x.IpEndPoint.Equals(y.IpEndPoint);
        }

        public int GetHashCode(Peer obj)
        {
            return obj.IpEndPoint.GetHashCode();
        }
    }
    #endregion
}