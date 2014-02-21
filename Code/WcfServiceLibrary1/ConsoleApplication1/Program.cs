using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApplication1.ServiceReference1;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            Thread.Sleep(2000);



            var client = new Service1Client();


            Console.WriteLine(client.GetData(2));
            


        }
    }
}
