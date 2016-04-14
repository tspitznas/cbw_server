using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace cbw_server
{
    class Program
    {
        private static TcpListener tcpListener;
        private static Thread listenThread;

        // main program entry point
        static void Main(string[] args)
        {
            // create a tcp/ip listener
            // set port number to listen on
            Int32 port = 8000;
            IPAddress localAddr = IPAddress.Any;
            tcpListener = new TcpListener(localAddr, port);

            Console.WriteLine("Waiting for connections on port " + port.ToString() + "... ");
            Console.WriteLine("");

            // create the main listening thread that runs the code found
            // in the function listen below
            listenThread = new Thread(new ThreadStart(listen));
            listenThread.Start();
        }

        // this function will run in a new thread
        // when a connection request is detected, a
        // new thread will be created to handle the connection
        static void listen()
        {
            // start the tcp listener
            tcpListener.Start();

            // loop forever listening for incoming connections
            // when there is a incoming connection, create a new thread
            // to handle the communication
            while (true)
            {
                // block, listening for incoming connection
                TcpClient client = tcpListener.AcceptTcpClient();

                // create a thread to handle communication
                Thread clientThread = new Thread(new ParameterizedThreadStart(handleComm));
                clientThread.Start(client);
            }
        }

        // this function will run in it's own thread
        // there should be 1 thread for each active connection
        static void handleComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] data = new byte[512];
            int bytesRead;

            Console.WriteLine("Connection received from: " + tcpClient.Client.RemoteEndPoint.ToString());
            Console.WriteLine("Line 2:" + tcpClient.Client.ProtocolType.ToString());


            // loop until the remote end closes the connection, or there is an error
            while (true)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = clientStream.Read(data, 0, 512);
                }
                catch
                {
                    Console.WriteLine("Error");
                    Console.WriteLine("");
                    break;
                }

                // a read of 0 bytes means the connection
                // was closed on the remote end
                if (bytesRead == 0)
                {
                    break;
                }

                ASCIIEncoding enc = new ASCIIEncoding();
                Console.WriteLine(enc.GetString(data, 0, bytesRead));
            }

            // close the connection on our side
            Console.WriteLine("Connection closed by remote end. Listening on port 8080.");
            Console.WriteLine("");
            tcpClient.Close();
        }
    }
}
