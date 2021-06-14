/**********************************************************************************************************************
 * 服务器 part 2/2
  *********************************************************************************************************************/
using System;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Collections.Generic;
class Program
{
    // 定义listen Socket
    static Socket listenfd;

    // 存储client Socket及状态信息 #<key, value>
    static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();


    static void Main(string[] args)
    {
        // Socket初始化
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Bind
        IPAddress iPAddress = IPAddress.Parse("192.168.1.160");
        IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 8888);
        listenfd.Bind(iPEndPoint);

        // Listen
        listenfd.Listen(0); // 最大连接长度，0为无限
        Console.WriteLine("[服务器]启动成功");

        // Accept
        listenfd.BeginAccept(AcceptCallback, listenfd);
        Console.ReadLine(); // 等待
    }

    // Accept回调
    private static void AcceptCallback(IAsyncResult ar)
    {
        try
        {
            Console.WriteLine("[服务器]Accept 有客户端连接，等待中");
            Socket _listenfd = ar.AsyncState as Socket; // 服务端，监听Socket
            Socket _clientfd = _listenfd.EndAccept(ar); // 客户端Socket

            // Clients列表，状态类
            ClientState _clientState = new ClientState(); // 状态类的实例
            _clientState.socket = _clientfd;
            clients.Add(_clientfd, _clientState);

            // Receive
            _clientfd.BeginReceive(_clientState.readBuff, 0, 1024, 0, ReceiveCallback, _clientState);
           
            listenfd.BeginAccept(AcceptCallback, listenfd); // 继续Accept
        }
        catch (SocketException e)
        {
            Console.WriteLine("[服务器]Socket Accept 失败，原因：" + e.ToString());
        }
    }

    // Receive回调
    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            ClientState _clientState = ar.AsyncState as ClientState;
            Socket _clientfd = _clientState.socket;
            int count = _clientfd.EndReceive(ar);
           
            if (count == 0) // 客户端关闭
            {
                _clientfd.Close();
                clients.Remove(_clientfd);
                Console.WriteLine("[服务器]有客户端断开/关闭");
                return;
            }

            string _recvStr = Encoding.UTF8.GetString(_clientState.readBuff, 0, count);
            Console.WriteLine("[服务器]接收到：" + _recvStr);

            // Send
            byte[] _sendBytes = Encoding.UTF8.GetBytes(_recvStr); // 广播的消息
            foreach (ClientState s in clients.Values) // 发送给所有连接上的客户端
            {
                s.socket.Send(_sendBytes);
            }

            _clientfd.BeginReceive(_clientState.readBuff, 0, 1024, 0, ReceiveCallback, _clientState); // agen Receive
        }
        catch (SocketException e)
        {
            Console.WriteLine("[服务器]Socket Receive 失败，原因：" + e.ToString());
        }
    }
}