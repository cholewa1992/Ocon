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
            var p = new Person();
            var p2 = new Person();
            p.i = 0;
            p.i = 10;
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
            return person.i > 0;
        }
    }
}
