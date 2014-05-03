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


            var client = new Client.Client(comHelper, new []{"s1"});
            client.SituationStateChangedEvent += (sender, eventArgs) => Console.WriteLine("State changed to " + eventArgs.Situation.State);
            Console.ReadLine();
        }
    }
}
