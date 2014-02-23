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



            var p = new Person(){i = 0, Name = "Person1"};
            var p2 = new Person(){i = 1, Name = "Person2"};
            
            var predicate = new Predicate<List<IEntity>>(Test);
            var context = new Context {ContextPredicate = predicate};
            Console.WriteLine();
            var contextFilter = new ContextFilter(context);
            
            contextFilter._entities.Add(p);
            contextFilter._entities.Add(p2);

            var widget = new Widget(contextFilter);
            widget.Start();


        }

        public static bool Test(List<IEntity> persons)
        {


            foreach (var person in persons)
            {
                if (person.GetType() == typeof (Person))
                    Console.WriteLine(person.GetType());

            }
            return false;
        }
    }
}
