﻿using System;
using System.Collections.Generic;
using System.Linq;
using Client;
using ContextawareFramework;
using ContextawareFramework.NetworkHelper;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ICommunicationHelper comHelper = TcpHelper.GetInstance();

            var p = new Predicate<ICollection<IEntity>>(entities => entities.OfType<Person>().Count() <= 3 );


            var situations = new ISituation[]
            {
                new Situation(p)
                {
                    Description = "3 Person present"
                } 
            };

            var client = new Client.Client(comHelper, situations);
            client.ContextEvent += (sender, eventArgs) => Console.WriteLine("State changed to " + eventArgs.State);
            Console.ReadLine();
        }
    }
}
