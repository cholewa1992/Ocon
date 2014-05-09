using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContextawareFramework;
using ContextawareFramework.NetworkHelper;

namespace TestCentral
{
    class Program
    {
        static void Main(string[] args)
        {

            var filter = new ContextFilter();

            filter.AddSituation(new Situation(TestForCloseupSituation){Name = "Closeup"});

            var central = new ContextCentral(filter, new TcpHelper(Console.Out), Console.Out);

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
    }
}
