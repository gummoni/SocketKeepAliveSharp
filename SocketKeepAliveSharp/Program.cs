using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace SocketKeepAliveSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new TcpClient())
            {
                client.EnableKeepAlive(3000, 5000); //5s周期,3sタイムアウト
            }
        }
    }

    /// <summary>
    /// TCP設定
    /// </summary>
    public static class TcpClientKeepAliveFeature
    {
        /// <summary>
        /// キープアライブ有効
        /// </summary>
        /// <param name="socket"></param>
        public static void EnableKeepAlive(this TcpClient tcpClient, uint keepAliveTimeMilliseconds, uint keepAliveIntervalMilliseconds)
        {
            var socket = tcpClient.Client;
            var tcpKeepalive = new byte[12];
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);     // keep-alive有効
            BitConverter.GetBytes((uint)1).CopyTo(tcpKeepalive, 0);                                 // onoffスイッチ
            BitConverter.GetBytes(keepAliveTimeMilliseconds).CopyTo(tcpKeepalive, 4);               // wait time.(ms)
            BitConverter.GetBytes(keepAliveIntervalMilliseconds).CopyTo(tcpKeepalive, 8);           // interval.(ms)
            socket.IOControl(IOControlCode.KeepAliveValues, tcpKeepalive, null);                    // keep-aliveのパラメータ設定
        }

        /// <summary>
        /// キープアライブ無効
        /// </summary>
        /// <param name="tcpClient"></param>
        public static void DisableKeepAlive(this TcpClient tcpClient)
        {
            var socket = tcpClient.Client;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, false);     // keep-alive無効
        }
    }



}
