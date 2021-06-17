/**********************************************************************************************************************
 * 客户端 #更新UGUI
  *********************************************************************************************************************/
using System;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
public class EchoClient : MonoBehaviour
{
    // 定义Socket
    private Socket socket;
   
    // UGUI
    public InputField inputField;
    public Text text;
   
    // 客户端计算机名、IP地址
    private string localNamePC = "";

    // 接收缓冲区
    private byte[] readBuff = new byte[1024];
    private string recvStr = "";


    // Connect
    public void Connection()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // 创建Socket对象
        socket.BeginConnect("192.168.1.160", 8888, ConnectionCallback, socket); // Connect
    }
   
    // Connect回调
    private void ConnectionCallback(IAsyncResult ar)
    {
        try
        {
            Socket _socket = ar.AsyncState as Socket;
            _socket.EndConnect(ar);
            Debug.Log("[客户端]Socket 连接成功");
           
            // Receive
            _socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, _socket);
        }
        catch (SocketException e)
        {
            Debug.Log("[客户端]Socket 连接失败，原因：" + e.ToString());
        }
    }

    // Receive回调
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket _socket = ar.AsyncState as Socket;
            int _count = _socket.EndReceive(ar); // end Receive
            string _recvStr = Encoding.UTF8.GetString(readBuff, 0, _count);
            recvStr = recvStr + '\n' + _recvStr; // 保留所有Receive到的消息
           
            _socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, _socket); // agen回调
        }
        catch (SocketException e)
        {
            Debug.Log("[客户端]Receive 失败，原因：" + e.ToString());
        }
    }

    // Send
    public void Send()
    {
        string _sendStr = inputField.text;
        _sendStr = localNamePC + "：" + _sendStr; // 客户端计算机名和消息
        byte[] _sendBytes = Encoding.UTF8.GetBytes(_sendStr);
        socket.BeginSend(_sendBytes, 0, _sendBytes.Length, 0, SendCallback, socket);
    }

    // Send回调
    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket _socket = ar.AsyncState as Socket;
            int _count = _socket.EndSend(ar); // end Send
            Debug.Log("[客户端]Send 成功，字节数：" + _count);
        }
        catch (SocketException e)
        {
            Debug.Log("[客户端]Send 失败，原因：" + e.ToString());
        }
    }
   
    void Start()
    {
        // 获取本机电脑名称
        localNamePC = Dns.GetHostName();
    }
   
    void Update()
    {
        // 更新UGUI
        text.text = recvStr;
    }
}