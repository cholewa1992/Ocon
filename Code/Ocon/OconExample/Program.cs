using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Ocon;
using Ocon.Entity;
using Ocon.Messages;
using Ocon.OconCommunication;
using Ocon.OconSerializer;
using Ocon.TcpCom;

namespace OconExample
{
    class Person : IEntity
    {
        public string WidgetName { get; set; }
        public Guid Id { get; set; }
        public Guid WidgetId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Helpers
            var serializer = new JsonNetAdapter();            
            var tcpCom = new TcpCom(serializer);
            var comHelper = new OconComHelper(tcpCom);
            
            //Client
            var client = new OconClient(comHelper, null, new Situation<bool>(c => c.OfType<Person>().Any(t => t.Description == "Jacob")));

            //Central
            var filter = new OconContextFilter();
            var central = new OconCentral(filter, comHelper);

            //Widget
            var widget = new OconWidget(comHelper);
            
            
            comHelper.Broadcast(DeviceType.Central);
            
            client.SituationStateChangedEvent += situation => Console.WriteLine(((Situation<bool>)situation).Value);

            Console.ReadLine();
            widget.Notify(new Person
            {
                Description = "Trine",
                Id = Guid.NewGuid(),
                Name = "test"
            });

            Console.ReadLine();

            widget.Notify(new Person
            {
                Description = "Jacob",
                Id = Guid.NewGuid(),
                Name = "test"
            });

            Console.ReadLine();





        }
    }
}
