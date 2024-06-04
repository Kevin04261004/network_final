using System.Diagnostics;
using System.Net;
using DYUtil;
using GameLogicServer.Datas;

namespace GameLogicServer
{
    public class LogicServer : SocketUDPServer<PacketDataInfo.EGameLogicPacketType>
    {
        public NetworkObjectManager networkObjectManager;
        public LogicServer(int portNum, PacketHandler<PacketDataInfo.EGameLogicPacketType> handler) : base(portNum, handler)
        {
            networkObjectManager = new NetworkObjectManager();
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
        public void CreateNetworkObjectRequirement(IPEndPoint endPoint, byte[] data)
        {
            Logger.Log($"{endPoint.Address}", "Create Network Object", ConsoleColor.White);

            CreateNetworkObjectData newObjData = MarshalingTool.ByteToStruct<CreateNetworkObjectData>(data);

            uint startID = networkObjectManager.CreateNetworkObject(newObjData);

            byte[] startIDBytes = BitConverter.GetBytes(startID);

            byte[] sendByte = new byte[data.Length + startIDBytes.Length];
            int offset = 0;
            send
            PacketData packet = new PacketData(PacketDataInfo.EGameLogicPacketType.Server_CreateNetworkObjectSuccess, data);
        }
        #endregion
    }
}
