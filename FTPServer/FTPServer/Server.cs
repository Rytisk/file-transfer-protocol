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

        public Server()
        {
            
        }

        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, 21);
            listener.Start();
            listener.BeginAcceptTcpClient(HandleAcceptTcpClient, listener);

            Console.ReadLine();
            //HandleAcceptTcpClient();
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
            Console.WriteLine("WTF");
            // TcpClient client = listener.AcceptTcpClient();
            TcpClient client = listener.EndAcceptTcpClient(result);
          

            NetworkStream stream = client.GetStream();

            using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                {
                    ClientConnection clientConnection = new ClientConnection(client);

                    clientConnection.HandleClient();
                }
            }
        }


    }
}
