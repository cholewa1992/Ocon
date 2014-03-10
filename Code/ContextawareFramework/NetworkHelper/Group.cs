using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace NetworkHelper
{
    public class Group
    {
        private readonly HashSet<Peer> _observers = new HashSet<Peer>(new PeerEquallityCompare());

        public void Send(string msg)
        {
            var toRemove = new List<Peer>();
            foreach (var observer in _observers)
            {
                try
                {
                    PrintObservers();
                    TcpHelper.SendTcpPackage(msg, observer.IpEndPoint);

                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                    toRemove.Add(observer);
                }
            }
            foreach (var peer in toRemove)
            {
                RemoveObserver(peer);
            }
        }

        public void AddObserver(Peer peer)
        {
            lock (_observers)
            {
                if (!_observers.Contains(peer))
                {
                    Console.WriteLine("Adding peer to group: " + peer.IpEndPoint);
                    _observers.Add(peer);
                }
            }
        }

        public void RemoveObserver(Peer peer)
        {
            lock (_observers)
            {
                Console.WriteLine("Removing peer from group: " + peer.IpEndPoint);
                _observers.Remove(peer);
            }
        }

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
}