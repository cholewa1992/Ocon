using System;
using System.Collections.Generic;
using System.Linq;
using Client;
using ContextawareFramework;
using ContextawareFramework.NetworkHelper;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ICommunicationHelper comHelper = new TcpHelper(Console.Out);

            var p = new Predicate<ICollection<IEntity>>(entities => entities.OfType<Person>().Count() <= 3 );

            var client = new Client.Client(comHelper);
            client.SituationStateChangedEvent += (sender, eventArgs) => Console.WriteLine("State changed to " + eventArgs.Situation.State);
            Console.ReadLine();
        }
    }
}
