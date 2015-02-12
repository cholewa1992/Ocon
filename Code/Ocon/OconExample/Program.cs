using System;
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
            var serializer = new JsonNetAdapter();

            Expression<Predicate<ICollection<IEntity>>> pre = (entities => entities.Any());
            var post = serializer.Deserialize <LambdaExpression>(serializer.Serialize(pre));
            Console.WriteLine(
            post.Compile().DynamicInvoke(new Collection<IEntity>()));
            Console.ReadLine();


            return;
            var tcpCom = new TcpCom(serializer);
            var comHelper = new OconComHelper(tcpCom);
            IOconPeer peer = null;
            tcpCom.RecievedMessageEvent += (msg, sender) =>
            {
                if (msg.Type == MessageType.Handshake) peer = sender;
                Console.WriteLine(msg.Type);
            };


            
            var client = new OconClient(comHelper, null, "test");
            var filter = new OconContextFilter();
            var central = new OconCentral(filter, comHelper);
            var widget = new OconWidget(comHelper);

            Console.ReadLine();


            comHelper.Broadcast(DeviceType.Central);
            
            //filter.AddSituation(new Situation("test", entities => entities.Any()));
            
            client.SituationStateChangedEvent += situation => Console.WriteLine(situation.Name);
            
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
