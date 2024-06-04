using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GameLogicServer;
using UnityEngine;

public class LogicClient : MonoBehaviour
{
    private readonly string serverIP = "127.0.0.1";
    private static readonly int PORT_NUM = 10000; 
    private Thread receiveThread;
    private NetworkManager networkManager;
    private void Awake()
    {
        networkManager = FindAnyObjectByType<NetworkManager>();
    }

    private void Start()
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
            networkManager.GameLogicUDPClientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress ipAddr = IPAddress.Parse(serverIP);
            networkManager.GameLogicServerEndPoint = new IPEndPoint(ipAddr, PORT_NUM);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
    private void ConnectToServer()
    {
        PacketData data = new PacketData(PacketDataInfo.EGameLogicPacketType.Client_TryConnectToServer);
        Debug.Log("[Client] Try Connect to UDP Server");
        networkManager.SendToServer(ESendServerType.GameLogic, data.ToPacket());
    }

    private void ExitGame()
    {
        PacketData data = new PacketData(PacketDataInfo.EGameLogicPacketType.Client_ExitGame);
        Debug.Log("[Client] Exit UDP Server");
        networkManager.SendToServer(ESendServerType.GameLogic, data.ToPacket());
    }
    private void ReceiveFromServer()
    {
        while (true)
        {
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        }
    }
}