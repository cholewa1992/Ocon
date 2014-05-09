using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using ContextawareFramework;
using ContextawareFramework.NetworkHelper;

namespace TestWidget
{
    public class Program
    {
        private static readonly List<Person> People = new List<Person>();
        private static int j = 0;

        public static void Main(string[] args)
        {
            var w = new Widget(new TcpHelper());
            w.StartDiscovery();
            var k = new Kinect();

            for (var i = 0; i < 6; i++)
            {
                People.Add(new Person
                {
                    Id = Guid.NewGuid()
                });
            }

            k.KinectEvent += (sender, eventArgs) =>
            {
                if (eventArgs == null) return;
                if (!eventArgs.PeoplePresent) return;

                
                for (var i = 0; i < eventArgs.NumberOfPeople; i++)
                {
                    People[i].Present = (eventArgs.NumberOfPeople > j);
                }

                

                foreach (var t in People)
                {
                    w.Notify(t);
                }

                j = eventArgs.NumberOfPeople;
                Console.WriteLine("j: "+j);
            };


            /*Task.Run(() =>
            {
                while (true)
                {
                    Console.Clear();
                    k.FireTestEvent();
                    Thread.Sleep(new Random().Next(5000, 10000));
                }
            });*/

            k.StartKinect();
            Console.ReadLine();
            k.Close();
        }
    }
}