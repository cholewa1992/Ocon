using System;
using System.Linq;
using Entities;
using Ocon;
using Ocon.OconCommunication;

namespace Centralization
{
    class Program
    {
        static void Main(string[] args)
        {

            //Instantiate a logging instance
            var log = Console.Out;

            //Instantiate an IOconCom implementation
            var tcpCom = new OconTcpCom(log);

            //Instantiate the context filter
            var oconFilter = new OconContextFilter(log);

            //Instantiate situations with names and predicates
            var closeupSituation = new Situation("Closeup", e => e.OfType<Person>().Count(p => p.Present) == 1);
            var standupSituation = new Situation("Standup", e => e.OfType<Person>().Count(p => p.Present) == 2);

            //Add the situations to the filter
            oconFilter.AddSituation(closeupSituation, standupSituation);

            //Instantiate the central
            var central = new OconCentral(oconFilter, tcpCom, log);

            //Initialize the central
            central.Initialize();

            Console.Read();
        }


    }
}
