using System;
using System.Collections.Generic;
using System.Linq;
using NetworkHelper;

namespace ContextawareFramework
{
    class Program
    {
        static void Main(string[] args)
        {

            var cf = new ContextFilter();
            cf.AddSituation(new Situation(), new Situation(), new Situation());

            var comHelper = TcpHelper.GetInstance();
            comHelper.Broadcast();
            comHelper.StartListen(comHelper.CommunicationPort); //Listening for sensor input
            comHelper.StartListen(comHelper.HandshakePort); //Listening for new clients
            Console.ReadLine();

            //var cc = new ContextCentral(cf, comHelper);



        }

        public static bool TestPredicate(ICollection<IEntity> entities)
        {
           

            Console.WriteLine(entities.OfType<Person>().First().Id);
            return entities.OfType<Person>().Any(t => t.Id == new Guid());
        }
    }
}
