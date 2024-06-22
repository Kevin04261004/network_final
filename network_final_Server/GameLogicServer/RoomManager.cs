using DYUtil;
using GameLogicServer.Datas;
using GameLogicServer.Datas.Database;
using System.Net;
using System.Net.Sockets;

namespace GameLogicServer
{
    public class RoomManager
    {
        public DBServer dbServer;

        public RoomManager(DBServer logicServer)
        {
            this.dbServer = logicServer;
        }

        public void CreateRoom(TcpClient client, DB_GameRoom gameRoom)
        {
            if (DatabaseConnector.HasRoomName(gameRoom.RoomName))
            {
                Logger.Log($"{client.Client.RemoteEndPoint}", "방 생성에 실패하였습니다.", ConsoleColor.Red);
                PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_CreateRoomFail);
                dbServer.Send(data.ToPacket(), client);
                return;
            }
            if (DatabaseConnector.TryCreateRoom(gameRoom))
            {
                Logger.Log($"{client.Client.RemoteEndPoint}", "방을 생성하였습니다.", ConsoleColor.DarkYellow);
                byte[] roomNameByte = new byte[DB_GameRoomInfo.ROOM_NAME_SIZE];
                
                MyEncoder.Encode(gameRoom.RoomName, roomNameByte, 0, roomNameByte.Length);
                PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_CreateRoomSuccess, roomNameByte);
                dbServer.Send(data.ToPacket(), client);
                DB_GameRoom curRoom = DatabaseConnector.GetGameRoom(gameRoom.RoomName);
                EnterRoom(client, curRoom);
            }
            else
            {
                Logger.Log($"{client.Client.RemoteEndPoint}", "방 생성에 실패하였습니다.", ConsoleColor.Red);
                PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_CreateRoomFail);
                dbServer.Send(data.ToPacket(), client);
            }
        }
        public void EnterRoom(TcpClient client, DB_GameRoom gameRoom)
        {
            uint roomId = gameRoom.RoomId;
            DB_RoomUserInfo roomUser = new DB_RoomUserInfo(roomId, dbServer.clients[client].Id, ((IPEndPoint)client.Client.RemoteEndPoint).ToString());
            byte[] gameRoomBytes = DB_GameRoomInfo.Serialize(gameRoom);
            if (DatabaseConnector.TryJoinRoom(roomUser))
            {
                Logger.Log($"{client.Client.RemoteEndPoint}", "방 입장에 성공하였습니다.", ConsoleColor.Green);
                PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_ClientEnterRoomSuccess, gameRoomBytes);
                dbServer.Send(data.ToPacket(), client);
            }
            else
            {
                Logger.Log($"{client.Client.RemoteEndPoint}", "방 입장에 실패하였습니다.", ConsoleColor.Red);
                PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_ClientEnterRoomFail, gameRoomBytes);
                dbServer.Send(data.ToPacket(), client);
            }
        }
        public void EnterRandomRoom(IPEndPoint endPoint)
        {
            Logger.Log($"{endPoint.Address}", "랜덤 방에 참가하였습니다.", ConsoleColor.DarkYellow);
            
        }
    }
}
