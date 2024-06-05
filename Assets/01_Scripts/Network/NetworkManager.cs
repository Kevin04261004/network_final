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
    private EndPoint gameLogicServerEndPoint = null;
    public EndPoint GameLogicServerEndPoint
    {
        get => gameLogicServerEndPoint;
        set
        {
            if (value != null)
            {
                gameLogicServerEndPoint = value;
            }

            if (value == null)
            {
                gameLogicServerEndPoint = null;
            }
        }
    }
    private Socket gameLogicUDPClientSock = null;
    public Socket GameLogicUDPClientSock
    {
        get => gameLogicUDPClientSock;
        set
        {
            if (value != null)
            {
                gameLogicUDPClientSock = value;
            }

            if (value == null)
            {
                gameLogicUDPClientSock = null;
            }
        }
    }
    private EndPoint dataBaseServerEndPoint = null;
    public EndPoint DataBaseServerEndPoint
    {
        get => dataBaseServerEndPoint;
        set
        {
            if (dataBaseServerEndPoint != null)
            {
                dataBaseServerEndPoint = value;
            }

            if (value == null)
            {
                dataBaseServerEndPoint = null;
            }
        }
    }
    private Socket dataBaseClientSock = null;
    public Socket DataBaseClientSock
    {
        get => dataBaseClientSock;
        set
        {
            if (dataBaseClientSock != null)
            {
                dataBaseClientSock = value;
            }

            if (value == null)
            {
                dataBaseClientSock = null;
            }
        }
    }
    private static readonly int GAME_LOGIC_SERVER_PORT_NUM = 10000;
    private static readonly int DATA_BASE_SERVER_PORT_NUM = 10001;
    public void SendToServer(ESendServerType serverType, byte[] packet)
    {
        Debug.Assert(packet != null, "들어온 Packet이 NULL입니다.");
        switch (serverType)
        {
            case ESendServerType.GameLogic:
                Debug.Log("Send GameLogic Packet");
                GameLogicUDPClientSock.SendTo(packet, GameLogicServerEndPoint);
                break;
            case ESendServerType.Database:
                Debug.Log("DB GameLogic Packet");
                DataBaseClientSock.SendTo(packet, DataBaseServerEndPoint);
                break;
            default:
                Debug.Assert(false, "Add Case!!!");
                break;
        }
    }
}
