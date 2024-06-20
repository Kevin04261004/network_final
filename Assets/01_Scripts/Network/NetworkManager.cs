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

    public EndPoint GameLogicServerEndPoint { get; set; } = null;
    public Socket GameLogicUDPClientSock { get; set; } = null;
    public TcpClient DatabaseTcpClient { get; set; } = null;
    public NetworkStream NetworkStream { get; set; } = null;
    public void SendToServer(ESendServerType serverType, byte[] packet)
    {
        Debug.Assert(packet != null, "들어온 Packet이 NULL입니다.");
        switch (serverType)
        {
            case ESendServerType.GameLogic:
                Debug.Assert(GameLogicUDPClientSock != null);
                Debug.Assert(GameLogicServerEndPoint != null);
                GameLogicUDPClientSock.SendTo(packet, GameLogicServerEndPoint);
                Debug.Log("Send GameLogic Packet");
                break;
            case ESendServerType.Database:
                Debug.Assert(DatabaseTcpClient != null, "DatabaseTcpClient is not initialized.");
                Debug.Assert(NetworkStream != null, "NetworkStream is not initialized.");
                NetworkStream.Write(packet, 0, packet.Length);
                NetworkStream.Flush();
                Debug.Log("Sending Database packet");
                break;
            default:
                Debug.Assert(false, "Add Case!!!");
                break;
        }
    }
}
