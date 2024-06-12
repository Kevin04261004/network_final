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
     protected override void OnApplicationQuit()
     {
         ExitGame();
         CloseServer();
     }
    // Send
    protected override void ConnectToServer()
     {
         var data = new PacketData<PacketDataInfo.EGameLogicPacketType>(PacketDataInfo.EGameLogicPacketType.Client_TryConnectToServer);
         Debug.Log("[Client] Try Connect to UDP Server");
         NetworkManager.Instance.SendToServer(ESendServerType.GameLogic, data.ToPacket());
     }
    // Send
    private void ExitGame()
     {
         var data = new PacketData<PacketDataInfo.EGameLogicPacketType>(PacketDataInfo.EGameLogicPacketType.Client_ExitGame);
         Debug.Log("[Client] Exit UDP Server");
         NetworkManager.Instance.SendToServer(ESendServerType.GameLogic, data.ToPacket());
     }
}