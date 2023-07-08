using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace message_client
{
    class Program
    {
        public static Socket socket;

        static void ReceiveThread()
        {
            while (true)
            {
                Thread.Sleep(50);
                byte[] buffer = new byte[300];
                int receive = socket.Receive(buffer, 0, buffer.Length, 0);
                Array.Resize(ref buffer, receive);

                Console.WriteLine(Encoding.Default.GetString(buffer));
            }
        }

        static void Main(string[] args)
        {
            int port = 1234;
            string ipServer = "127.0.0.1";
            Thread listener = new Thread(ReceiveThread);

            IPAddress ip = IPAddress.Parse(ipServer);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(ip, port));

            listener.Start();

            string userName = Console.ReadLine();

            while (true)
            {
                byte[] SendDataToServer = Encoding.Default.GetBytes("<"+ userName + ">: " + Console.ReadLine());
                socket.Send(SendDataToServer, 0, SendDataToServer.Length, 0);
            }
        }
    }
}
