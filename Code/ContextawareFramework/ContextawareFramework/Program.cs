using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

            var predicate = new Predicate<List<IEntity>>(Test);
            var context = new Context { ContextPredicate = predicate };
            Console.WriteLine();
            var contextFilter = new ContextFilter(context);

            contextFilter._entities.Add(p);
            contextFilter._entities.Add(p2);
            contextFilter._entities.Add(new Person(){i = 3});

            var widget = new Widget(contextFilter);
            widget.Start();


        }

        public static bool Test(IList<IEntity> persons)
        {
            // We might have to count multiple of same condition
            int personcount = 0;

            // Loop through all IEntities to check, not optimal
            foreach (var person in persons)
            {
                // If a person, cast and check properties
                if (person.GetType() == typeof(Person))
                {
                    var p = (Person)person;

                    if (p.i > 2 && personcount == 1)
                    {
                        return true;
                    }

                    personcount++;

                }


                if (person.GetType() == typeof(Room))
                    Console.WriteLine(person.GetType());

            }
            return false;
        }
    }
}
