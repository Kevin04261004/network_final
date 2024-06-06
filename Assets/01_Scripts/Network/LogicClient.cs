using System.Net;
using _01_Scripts.Network;
using UnityEngine;

public class LogicClient : UDPClient<PacketDataInfo.EGameLogicPacketType>
{
    protected override void Start()
    {
        base.Start();
    }
    protected override void SetAllHandlers()
    {
        
    }
    protected override void ProcessData(IPEndPoint serverIPEndPoint, PacketDataInfo.EGameLogicPacketType packetType, byte[] buffer)
    {
        Debug.Assert(packetType != PacketDataInfo.EGameLogicPacketType.None);
        
        GameLogicPacketHandler.Instance.ProcessPacket(serverIPEndPoint, packetType, buffer);
    }
    private void OnApplicationQuit()
     {
         ExitGame();
     }
    // Send
    protected override void ConnectToServer()
     {
         PacketData data = new PacketData(PacketDataInfo.EGameLogicPacketType.Client_TryConnectToServer);
         Debug.Log("[Client] Try Connect to UDP Server");
         NetworkManager.Instance.SendToServer(ESendServerType.GameLogic, data.ToPacket());
     }
    // Send
    private void ExitGame()
     {
         PacketData data = new PacketData(PacketDataInfo.EGameLogicPacketType.Client_ExitGame);
         Debug.Log("[Client] Exit UDP Server");
         NetworkManager.Instance.SendToServer(ESendServerType.GameLogic, data.ToPacket());
     }
}