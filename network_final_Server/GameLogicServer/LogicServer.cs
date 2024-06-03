using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using DYUtil;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameLogicServer
{
    public class LogicServer : SocketUDPServer<PacketDataInfo.EGameLogicPacketType>
    {
        public LogicServer(int portNum, PacketHandler<PacketDataInfo.EGameLogicPacketType> handler) : base(portNum, handler)
        {
        }


        protected override void ProcessData(IPEndPoint clientIPEndPoint, PacketDataInfo.EGameLogicPacketType packetType, byte[] buffer)
        {
            Debug.Assert(packetType != PacketDataInfo.EGameLogicPacketType.None);
            packetHandler.ProcessPacket(clientIPEndPoint, packetType, buffer);
        }

        protected override void SetAllHandlers()
        {
            packetHandler.SetHandler(PacketDataInfo.EGameLogicPacketType.Client_TryConnectToServer, ClientConnected);
            packetHandler.SetHandler(PacketDataInfo.EGameLogicPacketType.Client_ExitGame, ClientDisConnected);
        }

        #region Delegate PacketHandle Functions
        public void ClientConnected(IPEndPoint endPoint, byte[] data)
        {
            Logger.Log($"{endPoint.Address}", "Success Connect Server", ConsoleColor.Green);

            connectedClients.Add(endPoint);
        }
        public void ClientDisConnected(IPEndPoint endPoint, byte[] data)
        {
            Logger.Log($"{endPoint.Address}", "DisConnected", ConsoleColor.DarkMagenta);

            connectedClients.Remove(endPoint);
        }
        public void GetNetworkObjectID(IPEndPoint endPoint, byte[] data)
        {
            Logger.Log($"{endPoint.Address}", "Set Network Object ID", ConsoleColor.White);

        }
        #endregion
    }
}
