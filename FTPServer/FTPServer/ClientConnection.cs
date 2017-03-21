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
    class ClientConnection
    {
        private TcpClient _client;
        private TcpListener _passiveListener;

        private NetworkStream _networkStream;
        private StreamReader _reader;
        private StreamWriter _writer;
       

        private IPEndPoint _dataEndpoint;

        private string _username;
        private string _password;
        private string _transferType;

        public ClientConnection(TcpClient client)
        {
            _client = client;

            _networkStream = _client.GetStream();
            _reader = new StreamReader(_networkStream);
            _writer = new StreamWriter(_networkStream);
        }

        public void HandleClient()
        {
            _writer.WriteLine("220 Ready!");
            _writer.Flush();

            string line = null;

            while (!string.IsNullOrEmpty(line = _reader.ReadLine()))
            {
                string response = null;

                string[] command = line.Split(' ');

                string cmd = command[0].ToUpper();

                string argument = command.Length > 1 ? line.Substring(command[0].Length + 1) : null;

                if (string.IsNullOrWhiteSpace(argument))
                    argument = null;

                switch(cmd)
                {
                    case "USER":
                        response = CheckUsername(argument);
                        break;
                    case "PASS":
                        response = CheckPassword(argument);
                        break;
                    case "ACCT":
                        break;
                    case "CWD":
                        response = ChangeWorkingDirectory(argument);
                        break;
                    case "CDUP":
                        response = ChangeWorkingDirectory("..");
                        break;
                    case "SMNT":
                        response = "502 Command not implemented";
                        break;
                    case "REIN":
                        response = "502 Command not implemented";
                        break;
                    case "QUIT":
                        response = "221 Service closing control connection";
                        break;
                    case "PWD":
                        response = "257 \"/\" is current directory.";
                        break;
                    case "TYPE":
                        string[] args = argument.Split(' ');
                        response = Type(args[0], args.Length > 1 ? args[1] : null);
                        break;
                    case "PORT":
                        response = Port(argument);
                        break;
                    case "PASV":
                        response = Passive();
                        break;
                    default:
                        response = "502 Command not implemented";
                        break;
                }

                if(_client == null || !_client.Connected)
                {
                    break;
                }
                else
                {
                    _writer.WriteLine(response);
                    _writer.Flush();

                    if (response.StartsWith("221"))
                    {
                        break;
                    }
                }
            }
        }

        private string Type(string typeCode, string formatControl)
        {
            string response = "500 error";

            switch (typeCode)
            {
                case "I":
                    _transferType = typeCode;
                    response = "200 OK";
                    break;
                case "A":
                    response = "200 OK";
                    break;
                case "E":
                    break;
                case "L":
                    break;
                default:
                    response = "504 Command not implemented for that parameter.";
                    break;
            }

            if (formatControl != null)
            {
                switch (formatControl)
                {
                    case "N":
                        response = "200 Ok";
                        break;
                    case "T":
                        break;
                    case "C":
                        break;
                    default:
                        response = "504 Command not implemented for that parameter.";
                        break;
                }
            }

            return response;
        }
        
        private string Port(string hostPort)
        {
            string[] ipAndPort = hostPort.Split(',');

            byte[] ipAddress = new byte[4];
            byte[] port = new byte[2];

            for (int i = 0; i < 4; i++)
            {
                ipAddress[i] = Convert.ToByte(ipAndPort[i]);
            }

            for (int i = 4; i < 6; i++)
            {
                port[i - 4] = Convert.ToByte(ipAndPort[i]);
            }

            if (BitConverter.IsLittleEndian)
                Array.Reverse(port);

            BitConverter.ToInt16(port, 0);

            _dataEndpoint = new IPEndPoint(new IPAddress(ipAddress), BitConverter.ToInt16(port, 0));

            return "200 Data Connection Established";
        }

        private string Passive()
        {
            IPAddress localAddress = ((IPEndPoint)_client.Client.LocalEndPoint).Address;

            _passiveListener = new TcpListener(localAddress, 0);
            _passiveListener.Start();

            IPEndPoint passiveListenerEndpoint = (IPEndPoint)_passiveListener.LocalEndpoint;

            byte[] address = passiveListenerEndpoint.Address.GetAddressBytes();
            short port = (short)passiveListenerEndpoint.Port;

            byte[] portArray = BitConverter.GetBytes(port);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(portArray);

            return string.Format("227 Entering Passive Mode ({0},{1},{2},{3},{4},{5})", address[0], address[1], address[2], address[3], portArray[0], portArray[1]);
        }

        private string CheckUsername(string username)
        {
            _username = username;

            return "331 Username ok, need password";
        }

        private string CheckPassword(string password)
        {
            _password = password;

            if (true)
            {
                return "230 User logged in";
            }
            else
            {
                return "530 Not logged in";
            }
        }

        private string ChangeWorkingDirectory(string pathname)
        {
            return "250 Changed to new directory";
        }
    }
}
