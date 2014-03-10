using System;
using System.Collections.Generic;
using System.Linq;

namespace ContextawareFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new Context { ContextPredicate = TestPredicate };
            var contextFilter = new ContextFilter(context);
            Console.ReadLine();
        }

        public static bool TestPredicate(ICollection<IEntity> entities)
        {
            foreach (var room in entities.OfType<Room>())
            {
                Console.WriteLine(room);
            }

            Console.WriteLine(entities.OfType<Person>().First().i);
            return entities.OfType<Person>().Any(t => t.i > 7);
        }
    }
}
