using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    internal class Program
    {
        private static readonly IPAddress IpAddress = IPAddress.Loopback;//IPAddress.Parse("127.0.0.1")
        private static readonly int Port = 100;
        static void Main()
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.Unicode;
            Console.WriteLine("Client");

            Thread thread = new Thread(() =>
            {
                LoopFunction(IpAddress, Port);
            });
            thread.Start();

        }

        private static void LoopFunction(IPAddress ipAddress, int port)
        {
            while (true)
            {
                Console.Write("Mời nhập ký tự: ");
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("xESC");//fake char
                    SendToServer(ipAddress, port, keyInfo.KeyChar);
                    Console.Read();
                    break;
                }

                Console.WriteLine();
                SendToServer(ipAddress, port, keyInfo.KeyChar);
            }
        }

        private static void SendToServer(IPAddress ipAddress, int port, char key)
        {
            var serverEndPoint = new IPEndPoint(ipAddress, port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(serverEndPoint);
                var sendBuffer = new[] { (byte)key };
                socket.Send(sendBuffer);

                socket.Shutdown(SocketShutdown.Send);


                var binDataIn = new byte[512];

                var length = socket.Receive(binDataIn);

                var receiveBuffer = Encoding.Unicode.GetString(binDataIn, 0, length);
                //Console.WriteLine();
                Console.WriteLine(receiveBuffer);
                socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
