using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DYUtil;
using GameLogicServer.Datas;
using GameLogicServer.Datas.Database;

namespace GameLogicServer
{
    public class LogicServer : SocketUDPServer<PacketDataInfo.EGameLogicPacketType>
    {
        public NetworkObjectManager networkObjectManager;
        public LogicServer(int portNum, PacketHandler<PacketDataInfo.EGameLogicPacketType, IPEndPoint> handler) : base(portNum, handler)
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
            packetHandler.SetHandler(PacketDataInfo.EGameLogicPacketType.Client_RequireCreateNetworkObject, CreateNetworkObjectRequirement);
            packetHandler.SetHandler(PacketDataInfo.EGameLogicPacketType.Client_EnterRoom, ClientEnterRoom);
        
        }
        #region Delegate PacketHandle Functions
        private void ClientConnected(IPEndPoint endPoint, byte[] data)
        {
            Logger.Log($"{endPoint.Address}", "Success Connect Server", ConsoleColor.Green);
            connectedClients.Add(endPoint);
        }
        private void ClientDisConnected(IPEndPoint endPoint, byte[] data)
        {
            Logger.Log($"{endPoint.Address}", "DisConnected", ConsoleColor.DarkMagenta);

            connectedClients.Remove(endPoint);
        }
        private void CreateNetworkObjectRequirement(IPEndPoint endPoint, byte[] data)
        {
            Logger.Log($"{endPoint.Address}", "Create Network Object", ConsoleColor.White);
            CreateNetworkObjectData newObjData = MarshalingTool.ByteToStruct<CreateNetworkObjectData>(data);
            uint startID = networkObjectManager.CreateNetworkObject(newObjData);

            SendServerCreateNetworkObjectSuccess(data, startID, endPoint);
        }
        private void ClientEnterRoom(IPEndPoint endPoint, byte[] data)
        {
            Logger.Log($"{endPoint.Address}", "Enter Room", ConsoleColor.White);

            DB_RoomUserInfo roomUserInfo = DB_RoomUserInfoInfo.DeSerialize(data);
            roomUserInfo.IPEndPoint = endPoint.ToString();
            DatabaseConnector.TryJoinRoom(roomUserInfo);


        }
        #endregion
        private void SendServerCreateNetworkObjectSuccess(byte[] data, uint startID, IPEndPoint endPoint)
        {
            byte[] startIDBytes = BitConverter.GetBytes(startID);

            byte[] sendByte = new byte[data.Length + startIDBytes.Length];
            int offset = 0;
            Array.Copy(data, 0, sendByte, offset, data.Length);
            offset += data.Length;
            Array.Copy(startIDBytes, 0, sendByte, offset, startIDBytes.Length);
            offset += data.Length;

            PacketData packet = new PacketData(PacketDataInfo.EGameLogicPacketType.Server_CreateNetworkObjectSuccess, sendByte);
            Send(packet.ToPacket(), endPoint);
        }
    }
}
