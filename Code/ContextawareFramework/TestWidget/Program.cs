using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextawareFramework;
using Microsoft.Kinect;
using NetworkHelper;

namespace TestWidget
{
    public class Program
    {
        private static int _lastNumberOfPeople;
        private static readonly List<Person> _people = new List<Person>();  

        public static void Main(string[] args)
        {
            var w = new Widget.Widget(TcpHelper.GetInstance());
            var k = new Kinect();
            k.KinectEvent += (sender, eventArgs) =>
            {
                if (eventArgs == null) return;
                if (_lastNumberOfPeople == eventArgs.NumberOfPeople) return;

                _lastNumberOfPeople = eventArgs.NumberOfPeople;

                for (var i = 0; i < (_lastNumberOfPeople > _people.Count ? _lastNumberOfPeople : _people.Count ); i++)
                {
                    if (i < _lastNumberOfPeople)
                    {

                        if (_people.Count() < _lastNumberOfPeople)
                        {
                            var person = new Person();
                            _people.Add(person);
                        }

                        _people[i].Present = true;
                        w.Notify(_people[i]);
                    }
                    else
                    {
                        if (_people[i].Present)
                        {
                            _people[i].Present = false;
                            w.Notify(_people[i]);
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