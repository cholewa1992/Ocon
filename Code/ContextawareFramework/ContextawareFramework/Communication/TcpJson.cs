using System;
using Newtonsoft.Json;

namespace ContextawareFramework.Communication
{
    public class TcpJson
    {

        private readonly ContextFilter _contextFilter = new ContextFilter();


        public void Start(ICommunication com)
        {
            Console.WriteLine("Starting...");
            NetworkHelper.TcpHelper.IncommingTcpEvent += (sender, args) =>
            {
                try
                {
                    
                    var person = JsonConvert.DeserializeObject<Person>(args.Message);

                    Console.WriteLine("ok");

                    _contextFilter.TrackEntity(person);
                }
                catch
                {
                }
            };

            NetworkHelper.TcpHelper.StartTcpListen();
            NetworkHelper.TcpHelper.Broadcast();
            Console.WriteLine("Started");



        }


    }
}