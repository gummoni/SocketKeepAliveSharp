using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SocketKeepAliveSharp
{
    class Program
    {
        /// <summary>
        /// main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var listener = new TcpListener(IPAddress.Any, 5000);
            while (true)
            {
                Console.WriteLine("start");
                listener.Start();
                Console.WriteLine("waiting tcp client connection...");
                using var client = listener.AcceptTcpClient();
                listener.Stop();
                Communication(client);
            }
        }

        /// <summary>
        /// communication method
        /// </summary>
        /// <param name="client"></param>
        static void Communication(TcpClient client)
        {
            client.ReceiveTimeout = -1;
            client.EnableKeepAlive(3000, 5000);
            try
            {
                Console.WriteLine("connected.");
                while (true)
                {
                    var data = client.GetStream().ReadByte();
                    if (0 > data)
                    {
                        Console.WriteLine("disconnect");
                        break;
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine($"connection lost");
            }
        }
    }


    /// <summary>
    /// tcpclient extension method
    /// </summary>
    public static class TcpClientKeepAliveFeature
    {
        /// <summary>
        /// enable keepalive
        /// </summary>
        /// <param name="socket"></param>
        public static void EnableKeepAlive(this TcpClient tcpClient, uint keepAliveTimeMilliseconds, uint keepAliveIntervalMilliseconds)
        {
            var socket = tcpClient.Client;
            var tcpKeepalive = new byte[12];
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);     // enable keep-alive
            BitConverter.GetBytes((uint)1).CopyTo(tcpKeepalive, 0);                                 // switch on
            BitConverter.GetBytes(keepAliveTimeMilliseconds).CopyTo(tcpKeepalive, 4);               // wait time(ms)
            BitConverter.GetBytes(keepAliveIntervalMilliseconds).CopyTo(tcpKeepalive, 8);           // interval(ms)
            socket.IOControl(IOControlCode.KeepAliveValues, tcpKeepalive, null);                    // set keep-alive parameter
        }

        /// <summary>
        /// disable keepalive
        /// </summary>
        /// <param name="tcpClient"></param>
        public static void DisableKeepAlive(this TcpClient tcpClient)
        {
            var socket = tcpClient.Client;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, false);     // disable keep-alive
        }
    }
}
