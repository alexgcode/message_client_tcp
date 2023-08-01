using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace message_client
{
    public class Program
    {
        public static Socket socket;

        static void ReceiveThread()
        {
            while (true)
            {
                Thread.Sleep(50);
                byte[] buffer = new byte[500000];
                int receive = socket.Receive(buffer, 0, buffer.Length, 0);
                Array.Resize(ref buffer, receive);

                string rawMessage = Encoding.UTF8.GetString(buffer, 0, receive);
                Message msg = JsonConvert.DeserializeObject<Message>(rawMessage);

                Console.WriteLine(msg.data);
            }
        }

        public static byte[] ImageToByteArray()
        {
            string path = System.IO.Directory.GetCurrentDirectory();

            Image img = Image.FromFile(path + "\\image.jpg");

            byte[] bytes = (byte[])(new ImageConverter()).ConvertTo(img, typeof(byte[]));

            byte[] res = bytes.Take(bytes.Length).ToArray();//30
            
            //var list = Enumerable.Range(0, bytes.Length / 4)
            //                 .Select(i => BitConverter.ToInt32(bytes, i * 4))
            //                 .ToList();


            return res;
        }

        public static List<List<byte>> partition(List<byte> values, int chunkSize)
        {
            return values.Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        static void Main(string[] args)
        {
            int port = 1234;
            string ipServer = "192.168.88.1";
            Thread listener = new Thread(ReceiveThread);
            listener.Start();

            IPAddress ip = IPAddress.Parse(ipServer);
            
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(ip, port));

            

            Console.WriteLine("Ingrese nombre:");
            string userName = Console.ReadLine();

            Console.WriteLine("Ingrese numero:");
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

                Console.WriteLine("Ingrese target number: ");
                message.targetNumber = Console.ReadLine();

                Console.WriteLine("Ingrese mensaje: ");
                string mess = Console.ReadLine();  //message

                if(mess.Equals("2"))
                {
                    message.type = 2;
                    //message.data = BitConverter.ToString(ImageToByteArray());
                    //message.data = Encoding.ASCII.GetString(ImageToByteArray());

                    byte[] bytes = ImageToByteArray();

                    if(bytes.Length > 2220)
                    {
                        var listParts = partition(bytes.ToList(), 2220);

                        Console.WriteLine("sizes of parts: ");
                        int count = 1;
                        foreach (List<byte> item in listParts)
                        {
                            Message ms = new Message();
                            ms.senderNumber = number;
                            ms.targetNumber = message.targetNumber;
                            ms.type = 2;
                            ms.offset = count;
                            ms.part = listParts.Count;
                            ms.fileName = "test";
                            ms.fileExtension = "jpg";


                            Console.WriteLine($"size: {item.Count}");
                            string imagebase64 = Convert.ToBase64String(item.ToArray());
                            ms.data = imagebase64;

                            string jsonString = JsonConvert.SerializeObject(ms, Formatting.Indented);
                            //Console.WriteLine(jsonString);
                            byte[] SendDataToServer = Encoding.Default.GetBytes(jsonString);
                            Console.WriteLine($"sendSize: {SendDataToServer.Length}");

                            socket.Send(SendDataToServer, 0, SendDataToServer.Length, 0);

                            Thread.Sleep(100);
                            //socket.Disconnect(true);
                            count++;
                        }
                    }

                    //message.data = string.Empty;
                    //message.part = -1;

                    //string js = JsonConvert.SerializeObject(message, Formatting.Indented);

                    //byte[] sdts = Encoding.Default.GetBytes(js);
                    //socket.Send(sdts, 0, sdts.Length, 0);

                    /*
                    string imagebase64 = Convert.ToBase64String(bytes);
                    message.data = imagebase64; //.Substring(0, (int)(imagebase64.Length / 2));
                    Console.WriteLine($"size: {bytes.Length}");

                    string jsonString = JsonConvert.SerializeObject(message, Formatting.Indented);

                    byte[] SendDataToServer = Encoding.Default.GetBytes(jsonString);
                    socket.Send(SendDataToServer, 0, SendDataToServer.Length, 0);
                    */
                }
                else
                {
                    message.type = 1;
                    message.data = mess;

                    string jsonString = JsonConvert.SerializeObject(message, Formatting.Indented);

                    byte[] SendDataToServer = Encoding.Default.GetBytes(jsonString);
                    socket.Send(SendDataToServer, 0, SendDataToServer.Length, 0);
                }
            }
        }
    }
}
