using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextawareFramework;
using ContextawareFramework.NetworkHelper;

namespace TestWidget
{
    public class Program
    {
        private static int _lastNumberOfPeople;
        private static readonly List<Person> People = new List<Person>();  

        public static void Main(string[] args)
        {
            var w = new Widget.Widget(new TcpHelper(Console.Out));
            w.StartDiscovery();
            var k = new Kinect();
            k.KinectEvent += (sender, eventArgs) =>
            {
                if (eventArgs == null) return;
                if (_lastNumberOfPeople == eventArgs.NumberOfPeople) return;

                _lastNumberOfPeople = eventArgs.NumberOfPeople;

                for (var i = 0; i < (_lastNumberOfPeople > People.Count ? _lastNumberOfPeople : People.Count ); i++)
                {
                    if (i < _lastNumberOfPeople)
                    {

                        if (People.Count() < _lastNumberOfPeople)
                        {
                            var person = new Person();
                            People.Add(person);
                        }

                        People[i].Present = true;
                        w.Notify(People[i]);
                    }
                    else
                    {
                        if (People[i].Present)
                        {
                            People[i].Present = false;
                            w.Notify(People[i]);
                        }
                    }
                }
            };


            Task.Run(() =>
            {
                while (true)
                {
                    Console.Clear();
                    k.FireTestEvent();
                    Thread.Sleep(new Random().Next(5000,10000));
                }
            });

            //k.StartKinect();
            Console.ReadLine();
            k.Close();
        }
    }
}