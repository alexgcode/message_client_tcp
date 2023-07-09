using Newtonsoft.Json;
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

                string rawMessage = Encoding.UTF8.GetString(buffer, 0, receive);
                Message msg = JsonConvert.DeserializeObject<Message>(rawMessage);

                Console.WriteLine(msg.data);
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
            string number = Console.ReadLine();

            Message firstMSG = new Message();
            firstMSG.senderNumber = number;
            firstMSG.username = userName;
            string firstJsonString = JsonConvert.SerializeObject(firstMSG, Formatting.Indented);
            byte[] firstData = Encoding.Default.GetBytes(firstJsonString);
            socket.Send(firstData, 0, firstData.Length, 0);

            while (true)
            {
                Message message = new Message();
                message.senderNumber = number;
                message.username = userName;
                message.targetNumber = Console.ReadLine();
                message.data = Console.ReadLine();  //message

                string jsonString = JsonConvert.SerializeObject(message, Formatting.Indented);

                byte[] SendDataToServer = Encoding.Default.GetBytes(jsonString);
                socket.Send(SendDataToServer, 0, SendDataToServer.Length, 0);
            }
        }
    }
}
