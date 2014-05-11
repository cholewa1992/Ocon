using System;
using System.Collections.Generic;
using System.Linq;
using Ocon.OconCommunication;
using Ocon;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IOconCom comHelper = new OconTcpCom(Console.Out);


            var client = new OconClient(comHelper, Console.Out, "s1", "s2");
            client.SituationStateChangedEvent += (sender, eventArgs) => Console.WriteLine("State changed to " + eventArgs.State);
            Console.ReadLine();
        }
    }
}
