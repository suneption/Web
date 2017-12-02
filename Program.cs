using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace WebServer
{
    class Program
    {
        public static long _countOfRequests = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Server is starting");
            
            var serverStats = new ServerStatistics();
            using (var server = new Server(serverStats))
            {
                Console.WriteLine("Server is started");

                server.Listen().Wait();            
            }

            Console.WriteLine("Server closed");
        }
    }
}
