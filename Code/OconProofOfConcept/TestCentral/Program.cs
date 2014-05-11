using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Ocon;
using Ocon.Entity;
using Ocon.OconCommunication;

namespace TestCentral
{
    class Program
    {
        static void Main(string[] args)
        {

            var log = Console.Out;

            var tcpCom = new OconTcpCom(log);

            var OconFilter = new OconContextFilter(log);

            //Instantiate situations with names and predicates
            var closeupSituation = new Situation("Closeup", e => e.OfType<Person>().Count(p => p.Present) == 1);
            var standupSituation = new Situation("Standup", e => e.OfType<Person>().Count(p => p.Present) == 2);

            OconFilter.AddSituation(closeupSituation, standupSituation);

            var central = new OconCentral(OconFilter, tcpCom, log);

            central.Initialize();

            Console.Read();
        }


        public static bool TestForStandupMeetingSituation(ICollection<IEntity> entities)
        {

            int count = 0;

            foreach (var p in entities.OfType<Person>())
            {
                if (count >= 2) return true;

                if (p.Present) count++;
            }

            return false;
        }
    }
}
