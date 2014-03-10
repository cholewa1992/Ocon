using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MulticastListener
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
            ProtocolType.Udp);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 2001);
            s.Bind(ipep);

            IPAddress ip = IPAddress.Parse("224.5.6.7");

            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));
            while (true)
            {
                byte[] b = new byte[1024];
                s.Receive(b);
                string str = Encoding.ASCII.GetString(b, 0, b.Length);
                Console.WriteLine(str.Trim());
            }

        }
    }
}
