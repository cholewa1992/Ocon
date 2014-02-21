using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ContextawareFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Person(){i = 0, Name = "Person1"};
            var p2 = new Person(){i = 1, Name = "Person2"};
            
            var predicate = new Predicate<Person>(Test);
            var context = new Context();
            context.EntityPredicate = predicate;

            var contextFilter = new ContextFilter();
            contextFilter.AddContext(context);

            var widget = new Widget(contextFilter);
            widget.Start();


        }

        public static bool Test(Person person)
        {
            return person.i == 5;
        }
    }
}
