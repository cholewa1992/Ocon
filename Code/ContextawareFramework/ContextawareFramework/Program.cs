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
            cf.AddSituation(new Situation(t => t.OfType<Person>().Count(c => c.Present) > 2)
            {
                Name = "s1"
            });
            var comHelper = new TcpHelper(Console.Out);
            var cc = new ContextCentral(cf, comHelper);
            cc.Initialize();
            Console.ReadLine();

        }
    }
}
