using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TestClient
{
    class Program
    {
        static private TcpClient socketConnection;
        static private Thread clientReceiveThread;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ConnectToTcpServer();
            SendMessage("{class: RegisterUDP, connectionID: 1}");
        }

        static void ConnectToTcpServer()
        {
            try
            {
                clientReceiveThread = new Thread(new ThreadStart(ListenForData));
                clientReceiveThread.IsBackground = true;
                clientReceiveThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("On client connect exception " + e);
            }
        }

        static void ListenForData()
        {
            try
            {
                socketConnection = new TcpClient("localhost", 8052);
                Byte[] bytes = new Byte[1024];
                while (true)
                {
                    // Get a stream object for reading 				
                    using (NetworkStream stream = socketConnection.GetStream())
                    {
                        int length;
                        // Read incomming stream into byte arrary. 					
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);
                            // Convert byte array to string message. 						
                            string serverMessage = Encoding.ASCII.GetString(incommingData);
                            Console.WriteLine("server message received as: " + serverMessage);
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Console.WriteLine("Socket exception: " + socketException);
            }
        }

        static void SendMessage(string clientMessage)
        {
            if (socketConnection == null)
            {
                return;
            }
            try
            {
                // Get a stream object for writing. 			
                NetworkStream stream = socketConnection.GetStream();
                if (stream.CanWrite)
                {
                    // Convert string message to byte array.                 
                    byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                    // Write byte array to socketConnection stream.                 
                    stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                    Console.WriteLine("Client sent his message - should be received by server");
                }
            }
            catch (SocketException socketException)
            {
                Console.WriteLine("Socket exception: " + socketException);
            }
        }
    }
}

