using System;
using System.Globalization;
using System.Linq;
using Ocon;
using Ocon.Entity;
using Ocon.OconCommunication;
using Ocon.OconSerializer;
using Ocon.TcpCom;

namespace OconExample
{
    class Person : IEntity, IComparable<Person>
    {
        public Guid Id { get; set; }
        public Guid WidgetId { get; set; }
        public Beacon[] Beacons { get; set; }
        public int CompareTo(Person other)
        {
            return Id.CompareTo(other.Id);
        }
    }

    class Beacon
    {
        public Guid Id { get; set; }
        public int Rssi { get; set; }
        public double Distance { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {


            //Helpers
            var serializer = new JsonNetAdapter();            
            var tcpCom = new TcpCom(serializer);
            var comHelper = new OconComHelper(tcpCom);

            comHelper.EntityEvent += entity => Console.WriteLine("Got data");

            //Client

            var client = new OconClient(comHelper);
            var client2 = new OconClient(comHelper);
            client.Subscribe(new Situation<ComparableCollection<Person>>(c => new ComparableCollection<Person>(c.OfType<Person>())));

            client2.SituationStateChangedEvent += situation => Console.WriteLine("Client2!");

            //Central
            var filter = new OconContextFilter();
            var central = new OconCentral(filter, comHelper);
                       
            comHelper.Broadcast(DeviceType.Central,5);
            comHelper.DiscoveryEvent += peer => Console.WriteLine("Found peer {0}", peer.Id);
            client.SituationStateChangedEvent += situation =>
            {
                var list = situation as Situation<ComparableCollection<Person>>;
                if (list != null)
                {
                    foreach (var person in list.Value)
                    {
                        Console.WriteLine(person.Id);
                        foreach (var beacon in person.Beacons)
                        {
                            Console.WriteLine("\t" + beacon.Distance);
                        }
                    }
                }
            };

            Console.WriteLine("Now starting and listening for widgets");
            Console.ReadLine();
            client.UnsubscribeAll();
            Console.WriteLine("Unsubsribed all situations");
            Console.ReadLine();
        }
    }
}
