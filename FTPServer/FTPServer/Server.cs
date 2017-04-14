using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FTPServer
{
    class Server
    {
        private TcpListener listener;

        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);

        public Server()
        {
            
        }

        public void Start()
        {
            tcpClientConnected.Reset();

            Console.WriteLine("Waiting for a connection...");

            listener = new TcpListener(IPAddress.Any, 21);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(HandleAcceptTcpClient), listener);

            tcpClientConnected.WaitOne();
        }

        public void Stop()
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }

        private void HandleAcceptTcpClient(IAsyncResult result)
        {
            TcpClient client = listener.EndAcceptTcpClient(result);

            listener.BeginAcceptTcpClient(new AsyncCallback(HandleAcceptTcpClient), listener);

            NetworkStream stream = client.GetStream();

            using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                {
                    ClientConnection clientConnection = new ClientConnection(client);

                    clientConnection.HandleClient();
                }
            }
            tcpClientConnected.Set();
        }


    }
}
