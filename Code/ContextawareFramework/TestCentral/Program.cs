using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocon;
using Ocon.Entity;
using Ocon.OconCommunication;

namespace TestCentral
{
    class Program
    {
        static void Main(string[] args)
        {

            var filter = new ContextFilter();

            filter.AddSituation(new Situation(TestForCloseupSituation) { Name = "Closeup" });
            filter.AddSituation(new Situation(TestForCloseupSituation) { Name = "Standup" });

            var central = new OconCentral(filter, new OconTcpCom(Console.Out), Console.Out);

            central.Initialize();

            Console.Read();
        }

        public static bool TestForCloseupSituation(ICollection<IEntity> entities)
        {
            foreach (var p in entities.OfType<Person>())
            {
                if (p.Present) return true;
            }

            return false;
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
