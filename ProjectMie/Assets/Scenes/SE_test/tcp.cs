using System.IO;
using System.Net.Sockets;
using System.Collections;

using System.Net;
//using System.Net.Sockets; 包含于Net

using System.Collections.Generic;
using UnityEngine;

public class tcp : MonoBehaviour
{
    private IPAddress ipAddress;
    private const int port = 27015;
    private TcpListener tcpListener;    //服务端
    private TcpClient tcpClient;        //服务端
    private NetworkStream networkStream;    //利用它与客户端进行交流
    private BinaryReader binaryReader;  //读取数据？
    private BinaryWriter binaryWriter;  //写入数据？

    void Start()
    {
        /* 获取指定host的ip地址 */
        IPAddress[] listenerIP = Dns.GetHostAddresses("www.baidu.com");
        
        /* 创建TCP Listener对象，它需要一个公网IP和端口？ */
        tcpListener = new TcpListener(listenerIP[0], port);
        
        /* 开始监听 */
        tcpListener.Start();

        //TODO在新线程调用AcceptClientConnect
    }
    
    void AcceptClientConnect()
    {
        while(true)
        {
            try
            {
                /* 从 服务器 接收一个连接并赋给tcpClient，这是要从服务器接收一个客户端？？ */
                tcpClient = tcpListener.AcceptTcpClient();
                if(tcpClient != null)
                {
                    /* 初始化 Stream */
                    networkStream = tcpClient.GetStream();

                    /* 创建二进制读写对象，用于读写这个Stream */
                    binaryReader = new BinaryReader(networkStream);
                    binaryWriter = new BinaryWriter(networkStream);
                }
            }
            catch
            {
                Debug.LogError("无法连接到客户端。");
                break;
            }
        }
    }
}
