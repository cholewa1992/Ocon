using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Client;
using NetworkHelper;


namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ClientImp(TcpHelper.GetInstance());
            client.StartDiscovery();
            Console.ReadLine();
        }
    }
}
