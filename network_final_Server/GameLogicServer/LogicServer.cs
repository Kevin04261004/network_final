using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DYUtil;
using GameLogicServer.Datas;
using GameLogicServer.Datas.Database;
using MySqlX.XDevAPI;

namespace GameLogicServer
{
    public class LogicServer : SocketUDPServer<PacketDataInfo.EGameLogicPacketType>
    {
        public NetworkObjectManager networkObjectManager;
        public RoomHandler roomHandler;
        public LogicServer(int portNum, PacketHandler<PacketDataInfo.EGameLogicPacketType, IPEndPoint> handler) : base(portNum, handler)
        {
            networkObjectManager = new NetworkObjectManager();
            roomHandler = new RoomHandler(this);
        }
        protected override void ProcessData(IPEndPoint clientIPEndPoint, PacketDataInfo.EGameLogicPacketType packetType, byte[] buffer)
        {
            Debug.Assert(packetType != PacketDataInfo.EGameLogicPacketType.None);
            packetHandler.ProcessPacket(clientIPEndPoint, packetType, buffer);
        }
        protected override void SetAllHandlers()
        {
            packetHandler.SetHandler(PacketDataInfo.EGameLogicPacketType.Client_TryConnectToServer, ClientConnected);
            packetHandler.SetHandler(PacketDataInfo.EGameLogicPacketType.Client_ExitGameLogic, ClientDisConnected);
            packetHandler.SetHandler(PacketDataInfo.EGameLogicPacketType.Client_RequireCreateNetworkObject, CreateNetworkObjectRequirement);
            packetHandler.SetHandler(PacketDataInfo.EGameLogicPacketType.Client_EnterRoom, ClientEnterRoom);
            packetHandler.SetHandler(PacketDataInfo.EGameLogicPacketType.Client_ExitRoom, ClientExitRoom);
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

            SendClientEnter(roomUserInfo);
        }
        /// <summary>
        /// TCP로 방에 접속하고, UDP에서 방에서 나옴.
        /// </summary>
        private void ClientExitRoom(IPEndPoint endPoint, byte[] data)
        {
            Logger.Log($"{endPoint.Address}", "Exit Room", ConsoleColor.White);

            DatabaseConnector.ExitRoom(endPoint);

            DB_RoomUserInfoInfo.TryParseIPEndPoint(endPoint, out string str);
            Debug.Assert(str != null);
            DB_RoomUserInfo userInfo = DatabaseConnector.GetData<DB_RoomUserInfo>($"IPEndPoint = \'{str}\'")[0];
            Debug.Assert(userInfo != null);

            SendClientExit(userInfo);
        }
        #endregion
        private void SendClientEnter(DB_RoomUserInfo userInfo)
        {
            uint roomId = userInfo.RoomId;
            List<DB_RoomUserInfo> clients = DatabaseConnector.GetData<DB_RoomUserInfo>($"roomId = {roomId}");
            Debug.Assert(clients != null);

            byte[] sendData = new byte[clients.Count * DB_RoomUserInfoInfo.GetByteSize()];
            int offset = 0;
            for (int i = 0; i < clients.Count; ++i)
            {
                DB_RoomUserInfoInfo.TryParseIPEndPoint(clients[i].IPEndPoint, out IPEndPoint endPoint);
                roomHandler.ClientEnterRoom(roomId, endPoint);
                byte[] dataPerClient = DB_RoomUserInfoInfo.Serialize(clients[i]);
                Array.Copy(dataPerClient, 0, sendData, offset, dataPerClient.Length);
            }
            PacketData packetData = new PacketData(PacketDataInfo.EGameLogicPacketType.Server_P2P_ClientEnter, sendData);
            roomHandler.SendToRoomClients(roomId, packetData.ToPacket());
        }
        private void SendClientExit(DB_RoomUserInfo userInfo)
        {
            uint roomId = userInfo.RoomId;
            List<DB_RoomUserInfo> clients = DatabaseConnector.GetData<DB_RoomUserInfo>($"roomId = {roomId}");
            Debug.Assert(clients != null);

            Debug.Assert(userInfo.IPEndPoint != null);
            DB_RoomUserInfoInfo.TryParseIPEndPoint(userInfo.IPEndPoint, out IPEndPoint endPoint);
            roomHandler.ClientExitRoom(roomId, endPoint);

            byte[] data = DB_RoomUserInfoInfo.Serialize(userInfo);
            PacketData packetData = new PacketData(PacketDataInfo.EGameLogicPacketType.Server_P2P_ClientExit, data);
            roomHandler.SendToRoomClients(roomId, packetData.ToPacket());
        }
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
