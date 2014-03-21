using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ContextawareFramework;
using NetworkHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContextawareFramework
{
    

    class Program
    {
        static void Main(string[] args)
        {


            var cf = new ContextFilter();
            cf.AddSituation(new Situation(), new Situation(), new Situation());

            var comHelper = TcpHelper.GetInstance();
            comHelper.Broadcast();
            comHelper.StartListen(comHelper.CommunicationPort); //Listening for sensor input
            comHelper.StartListen(comHelper.HandshakePort); //Listening for new clients
            Console.ReadLine();



            comHelper.IncommingTcpEvent += (sender, eventArgs) =>
            {
                var entityType = GetEntityTypeEnum(eventArgs.Message);

                IEntity receivedEntity = null;

                switch (entityType)
                {
                    case EntityType.Person:
                        receivedEntity = JsonConvert.DeserializeObject<Person>(eventArgs.Message);
                        break;

                    case EntityType.Room:
                        receivedEntity = null;
                        break;
                }


                cf.TrackEntity(receivedEntity);


            };

            var cc = new ContextCentral(cf, comHelper);



        }

        public static bool TestPredicate(ICollection<IEntity> entities)
        {
           

            Console.WriteLine(entities.OfType<Person>().First().Id);
            return entities.OfType<Person>().Any(t => t.Id == new Guid());
        }

        public static EntityType GetEntityTypeEnum(string json)
        {
            string typeString = JObject.Parse(json)["$type"].ToString();

            string[] trim = typeString.Split('.', ',');


            return ParseEnum<EntityType>(trim[1]);
        }

        public static string GetEntityTypeString(string json)
        {
            string typeString = JObject.Parse(json)["$type"].ToString();

            string[] trim = typeString.Split('.', ',');

            return trim[1];
        }


        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
