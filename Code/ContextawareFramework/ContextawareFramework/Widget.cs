using System;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace ContextawareFramework
{
    public class Widget
    {

        private ContextFilter _contextFilter;
        private Person testPerson = new Person();

        public Widget(ContextFilter cf)
        {
            _contextFilter = cf;
        }


        public void Start()
        {
            var t = new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    Thread.Sleep(new Random().Next(2000));
                    Update(new Random().Next(10));
                }
                
            }));

            t.Start();


        }

        public void Update(int i)
        {
            

            testPerson.i = i;
            _contextFilter.EntitiesUpdated();
        }

    }
}