using System;
using System.Collections;
using System.Net;
using _01_Scripts.Network;
using UnityEngine;

public class LogicClient : UDPClient<PacketDataInfo.EGameLogicPacketType>
{
    public static LogicClient Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        GameLogicPacketHandler.Instance.SetHandler(PacketDataInfo.EGameLogicPacketType.Server_Temp, Temp);
    }

    private void Temp(IPEndPoint endPoint, byte[] data)
    {
        ;
    }
    protected override void ProcessData(IPEndPoint serverIPEndPoint, Int16 packetType, byte[] buffer)
    {
        PacketDataInfo.EGameLogicPacketType type = (PacketDataInfo.EGameLogicPacketType)packetType;
        Debug.Assert(type != PacketDataInfo.EGameLogicPacketType.None);
        
        GameLogicPacketHandler.Instance.ProcessPacket(serverIPEndPoint, type, buffer);
    }
    protected override void OnApplicationQuit()
     {
         ExitGame();
         CloseServer();
     }
     protected override void ConnectToServer() 
     {
         var packetData = new PacketData<PacketDataInfo.EGameLogicPacketType>(PacketDataInfo.EGameLogicPacketType.Client_TryConnectToServer);
         Debug.Log("[Client] Try Connect to UDP Server");
         NetworkManager.Instance.SendToServer(ESendServerType.GameLogic, packetData.ToPacket());
     }

     public void UDPConnectToRoom()
     {
         DB_GameRoom curGameRoom = CurrentGameRoomData.Instance.GameRoom;
         Debug.Assert(NetworkManager.Instance.GameLogicUDPClientSock != null);
         DB_RoomUserInfo roomUserInfo =
             new DB_RoomUserInfo(curGameRoom.RoomId, UserGameData.Instance.GameData.Id, "clientIPEndPoint", false, 0, false);
         var data = DB_RoomUserInfoInfo.Serialize(roomUserInfo);
         var packetData =
             new PacketData<PacketDataInfo.EGameLogicPacketType>(PacketDataInfo.EGameLogicPacketType.Client_EnterRoom, data);
         NetworkManager.Instance.SendToServer(ESendServerType.GameLogic, packetData.ToPacket());
     }
     
    private void ExitGame()
    {
        var data = new PacketData<PacketDataInfo.EGameLogicPacketType>(PacketDataInfo.EGameLogicPacketType.Client_ExitRoom);
        Debug.Log("[Client] Exit Room");
        NetworkManager.Instance.SendToServer(ESendServerType.GameLogic, data.ToPacket());
        data = new PacketData<PacketDataInfo.EGameLogicPacketType>(PacketDataInfo.EGameLogicPacketType.Client_ExitGameLogic);
        Debug.Log("[Client] Exit UDP Server");
        NetworkManager.Instance.SendToServer(ESendServerType.GameLogic, data.ToPacket());
    }
}