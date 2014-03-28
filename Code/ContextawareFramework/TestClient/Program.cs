using System;
using Client;
using ContextawareFramework.NetworkHelper;

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
