using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using ContextawareFramework;
using ContextawareFramework.NetworkHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContextawareFramework
{
    

    class Program
    {
        static void Main(string[] args)
        {


            var cf = new ContextFilter();
            var comHelper = TcpHelper.GetInstance();
            var cc = new ContextCentral(cf, comHelper);
            cc.Initialize();
            Console.ReadLine();

        }

        public static bool TestPredicate(ICollection<IEntity> entities)
        {
            Console.WriteLine(entities.OfType<Person>().First().Id);
            return entities.OfType<Person>().Any(t => t.Id == new Guid());
        }
    }
}
