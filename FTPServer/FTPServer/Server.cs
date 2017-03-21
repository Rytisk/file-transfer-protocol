using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
            listener.BeginAcceptTcpClient(HandleAcceptTcpClient, listener);

            NetworkStream stream = client.GetStream();

            using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                {
                    writer.WriteLine("220 Ready!");
                    writer.Flush();

                    string line = null;

                    while(!string.IsNullOrEmpty(line = reader.ReadLine()))
                    {
                        Console.WriteLine(line);
                        writer.WriteLine("502 I don't know");
                        writer.Flush();
                    }
                }
            }
        }


    }
}
