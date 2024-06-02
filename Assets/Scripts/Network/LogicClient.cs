using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class LogicClient : MonoBehaviour
{
    private readonly string serverIP = "127.0.0.1";
    private static readonly int PORT_NUM = 10000; 
    private EndPoint serverEndPoint;
    private Socket clientSock;
    private Thread receiveThread;
    private void Awake()
    {
        Init();
        ConnectToServer();
        receiveThread = new Thread(ReceiveFromServer)
        {
            IsBackground = true
        };
        receiveThread.Start();
    }

    private void OnApplicationQuit()
    {
        ExitGame();
    }

    private void Init()
    {
        try
        {
            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress ipAddr = IPAddress.Parse(serverIP);
            serverEndPoint = new IPEndPoint(ipAddr, PORT_NUM);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public void SendToServer(byte[] packet)
    {
        clientSock.SendTo(packet, serverEndPoint);
    }
    private void ConnectToServer()
    {
        PacketData data = new PacketData(PacketDataInfo.EPacketType.Client_TryConnectToServer);
        Debug.Log("[Client] Try Connect to UDP Server");
        SendToServer(data.ToPacket());
    }

    private void ExitGame()
    {
        PacketData data = new PacketData(PacketDataInfo.EPacketType.Client_ExitGame);
        Debug.Log("[Client] Exit UDP Server");
        SendToServer(data.ToPacket());
    }
    private void ReceiveFromServer()
    {
        while (true)
        {
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        }
    }
}