using System;
using System.Collections.Generic;
using System.Linq;

namespace ContextawareFramework
{
    class Program
    {
        static void Main(string[] args)
        {

            //string json = JsonConvert.SerializeObject(new Context() { EntityPredicate = new Predicate<List<Person>>(Test) });
            //Console.WriteLine(json);
            //var o = JsonConvert.DeserializeObject<Context>(json);
            //Console.WriteLine(o.EntityPredicate.ToString());

            var p = new Person() { i = 3, Name = "Person1" };
            var p2 = new Room() { Name = "Room1" };

            var context = new Context { ContextPredicate = TestPredicate };
            Console.WriteLine();
            var contextFilter = new ContextFilter(context);

            contextFilter._entities.Add(p);
            contextFilter._entities.Add(p2);
            contextFilter._entities.Add(new Person(){i = 3});

            var widget = new Widget(contextFilter);
            widget.Start();


        }

        public static bool TestPredicate(ICollection<IEntity> entities)
        {
            foreach (var room in entities.OfType<Room>())
            {
                Console.WriteLine(room);
            }

            return entities.OfType<Person>().Any(t => t.i > 2);
        }
    }
}
