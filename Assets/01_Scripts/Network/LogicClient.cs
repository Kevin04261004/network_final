using System.Collections;
using System.Net;
using _01_Scripts.Network;
using UnityEngine;

public class LogicClient : UDPClient<PacketDataInfo.EGameLogicPacketType>
{
    protected override void ProcessData(IPEndPoint serverIPEndPoint, PacketDataInfo.EGameLogicPacketType packetType, byte[] buffer)
    {
        Debug.Assert(packetType != PacketDataInfo.EGameLogicPacketType.None);
        
        GameLogicPacketHandler.Instance.ProcessPacket(serverIPEndPoint, packetType, buffer);
    }
    
    private void Start()
    {
        UDPConnectToRoom();
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

         if (CurrentGameRoomData.Instance.GameRoom == null)
         {
             return;
         }
     }

     private void UDPConnectToRoom()
     {
         DB_GameRoom curGameRoom = CurrentGameRoomData.Instance.GameRoom;
         Debug.Assert(NetworkManager.Instance.GameLogicUDPClientSock != null);
         DB_RoomUserInfo roomUserInfo =
             new DB_RoomUserInfo(curGameRoom.RoomId, UserGameData.Instance.GameData.Id, "clientIPEndPoint");
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