using System;
using System.Collections.Generic;
using System.Linq;
using ContextawareFramework.Communication;

namespace ContextawareFramework
{
    class Program
    {
        static void Main(string[] args)
        {

            new ContextFilter().AddSituation(new Situation(), new Situation(), new Situation());

            

            new TcpJson().Start(null);
            Console.ReadLine();


        }

        public static bool TestPredicate(ICollection<IEntity> entities)
        {
           

            Console.WriteLine(entities.OfType<Person>().First().Id);
            return entities.OfType<Person>().Any(t => t.Id == new Guid());
        }
    }
}
