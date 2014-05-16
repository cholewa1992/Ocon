using System;
using System.Collections.Generic;
using Entities;
using Ocon;
using Ocon.OconCommunication;

namespace KinectEntitySensor
{
    public class Program
    {
        private static readonly List<Person> People = new List<Person>();
        private static int j = 0;

        public static void Main(string[] args)
        {

            //Instantiate a logging instance
            var log = Console.Out;
            //for file logging: new StreamWriter("/file/path/here");

            //Instantiate an IOconCom implementation
            var com = new OconTcpCom(log);

            //Instantiate widget
            var widget = new OconWidget(com, log);

            //Start searching for a central with the given IOconCom implementation
            widget.StartDiscovery();

            //Pass an entity to be added/updated at the central
            var entity = new Person() { Name = "Mat", Present = true };
            widget.Notify(entity);



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
                    widget.Notify(t);
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