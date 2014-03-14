using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using ContextawareFramework;
using Microsoft.Kinect;

namespace TestWidget
{
    public class Program
    {
        private static int _lastNumberOfPeople;
        private static List<Person> _people = new List<Person>();  

        public static void Main(string[] args)
        {
            var w = new Widget.Widget();

            var k = new Kinect();
            k.KinectEvent += (sender, eventArgs) =>
            {
                if (eventArgs == null) return;
                Console.WriteLine(eventArgs.NumberOfPeople);

                if (_lastNumberOfPeople == eventArgs.NumberOfPeople) return;

                _lastNumberOfPeople = eventArgs.NumberOfPeople;

                for (var i = 0; i < (_lastNumberOfPeople > _people.Count ? _lastNumberOfPeople : _people.Count ); i++)
                {
                    if (i < _lastNumberOfPeople)
                    {
                        _people.Add(new Person
                        {
                            Id = i,
                            Present = true,
                            WidgetId = w.WidgetId
                        });
                    }
                    else
                    {
                        _people[i].Present = false;
                    }
                }
            };

            k.StartKinect();
            Console.ReadLine();
            k.Close();
        }
    }
}