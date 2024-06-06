using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public enum ESendServerType
{
    GameLogic,
    Database,
}

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private EndPoint gameLogicServerEndPoint = null;
    public EndPoint GameLogicServerEndPoint { get; set; } = null;

    public Socket GameLogicUDPClientSock { get; set; } = null;

    public TcpClient DatabaseTcpClient { get; set; } = null;
    public NetworkStream NetworkStream { get; set; } = null;

    private static readonly int GAME_LOGIC_SERVER_PORT_NUM = 10000;
    private static readonly int DATA_BASE_SERVER_PORT_NUM = 10001;
    public void SendToServer(ESendServerType serverType, byte[] packet)
    {
        Debug.Assert(packet != null, "들어온 Packet이 NULL입니다.");
        switch (serverType)
        {
            case ESendServerType.GameLogic:
                Debug.Assert(GameLogicUDPClientSock != null);
                Debug.Assert(GameLogicServerEndPoint != null);
                Debug.Log("Send GameLogic Packet");
                GameLogicUDPClientSock.SendTo(packet, GameLogicServerEndPoint);
                break;
            case ESendServerType.Database:
                Debug.Assert(DatabaseTcpClient != null);
                Debug.Assert(DatabaseTcpClient != null);
                Debug.Log("DB GameLogic Packet");
                //..SendTo(packet, DataBaseServerEndPoint);
                break;
            default:
                Debug.Assert(false, "Add Case!!!");
                break;
        }
    }
}
