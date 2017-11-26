using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

namespace WebServer
{
    class Program
    {
        public const string IpAddress = "127.0.0.1";
        public const int Port = 10001;
        public static long _countOfRequests = 0;

        static void Main(string[] args)
        {
            var timer = new System.Timers.Timer(1000);
            timer.Start();
            timer.Elapsed += (sender, e) => {
                var countOfRequests = Interlocked.Exchange(ref _countOfRequests, 0);
                Console.WriteLine(countOfRequests);
            };

            Console.WriteLine("Server is starting");

            using(var socket = Initialize()) 
            {
                Console.WriteLine("Server started");

                while(true) 
                {                    
                    var clientSocket = socket.Accept();
                    if (!ThreadPool.SetMaxThreads(2, 2)) { throw new NotImplementedException(); }
                    ThreadPool.QueueUserWorkItem((state) => { 
                        HandleClient(state);
                        Interlocked.Increment(ref _countOfRequests);
                    }, clientSocket);
                }
            }

            Console.WriteLine("Server closed");
        }

        public static Socket Initialize() 
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Parse(IpAddress), Port));
            socket.Listen(10);

            return socket;
        }

        public static void HandleClient(object state) {
            Socket clientSocket = null;
            try 
            {
                clientSocket = state as Socket;

                if (clientSocket == null) 
                {
                    throw new ArgumentException();
                }

                var inputByteBuffer = new byte[1024];
                // clientSocket.BeginReceive(inputByteBuffer, 0, inputByteBuffer.Length, 
                //     SocketFlags.None, 
                //     (asyncResult) => { 
                //         try 
                //         {
                //             var countOfInputBytes = clientSocket.EndReceive(asyncResult); 
                //             // var message = Encoding.UTF8.GetString(inputByteBuffer.Take(countOfInputBytes).ToArray());
                //             // var inputMessage = message.Trim();
                //             // var outputMessageBytes = Encoding.UTF8.GetBytes(inputMessage.ToUpper());
                //             Thread.Sleep(1000);
                //             var outputMessageBytes = new byte[] {};
                //             clientSocket.Send(outputMessageBytes);
                //         }
                //         finally
                //         {
                //             if (clientSocket != null) 
                //             {
                //                 clientSocket.Shutdown(SocketShutdown.Both);
                //                 clientSocket.Close();
                //             }
                //         }
                //     }, null);


                var countOfInputBytes = clientSocket.Receive(inputByteBuffer);

                // var message = Encoding.UTF8.GetString(inputByteBuffer.Take(countOfInputBytes).ToArray());

                // var inputMessage = message.Trim();
                // //Console.WriteLine($"Client received: {inputMessage}");

                // var outputMessageBytes = Encoding.UTF8.GetBytes(inputMessage.ToUpper());
                Thread.Sleep(1000);

                var outputMessageBytes = new byte[] {};
                clientSocket.Send(outputMessageBytes);
            } 
            finally 
            {
                if (clientSocket != null) 
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
        }
    }
}
