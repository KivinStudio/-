/**********************************************************************************************************************
 * 服务器 part 1/2
 * 用以存储客户端状态信息，供服务器Program class调用
  *********************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
/// <summary>
/// 保存所有连接上来的客户端信息
/// </summary>
class ClientState
{
    public Socket socket;
    public byte[] readBuff = new byte[1024];
}
