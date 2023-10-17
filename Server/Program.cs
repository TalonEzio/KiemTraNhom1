using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    internal class Program
    {
        private static readonly IPAddress IpAddress = IPAddress.Loopback;
        private static readonly int Port = 100;

        static void Main()
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.Unicode;
            Console.WriteLine("Server");
            TcpListener myListener = new TcpListener(IpAddress, Port);

            Thread thread = new Thread(
                () =>
                {
                    HandleClient(myListener);
                }
                );
            thread.Start();
        }

        static void HandleClient(TcpListener myListener)
        {
            try
            {
                myListener.Start();

                Console.WriteLine("Server đang chạy ở : " + myListener.LocalEndpoint);
                Console.WriteLine("Đang chờ kết nối.....");


                while (true)
                {
                    Socket socket = myListener.AcceptSocket();

                    Console.WriteLine("Kết nối từ " + socket.RemoteEndPoint);

                    byte[] binDataIn = new byte[512];
                    socket.Receive(binDataIn);

                    var receive = Encoding.Unicode.GetString(binDataIn)[0];
                    var receiveView = receive == '\u001b' ? "Esc" : receive.ToString();
                    Console.WriteLine($"Dữ liệu gửi từ client: {receiveView}");
                    byte[] binOut;
                    if (receive == '\u001b')
                    {
                        binOut = Encoding.Unicode.GetBytes("Đã nhận lệnh dừng chương trình");
                        socket.Send(binOut);
                        Console.WriteLine("Dừng chương trình!");
                        socket.Close();
                        break;
                    }

                    binOut = Encoding.Unicode.GetBytes($"Đã nhận được {receive} từ client");
                    socket.Send(binOut);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }
    }
}

