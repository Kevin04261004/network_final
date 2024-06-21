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

        public void CreateRoom(TcpClient client, string roomName)
        {
            if (DatabaseConnector.HasRoomName(roomName))
            {
                Logger.Log($"{client.Client.RemoteEndPoint}", "방 생성에 실패하였습니다.", ConsoleColor.Red);
                PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_CreateRoomFail);
                dbServer.Send(data.ToPacket(), client);
                return;
            }
            DB_GameRoom room = new DB_GameRoom(roomName);
            if (DatabaseConnector.TryCraeteRoom(room))
            {
                Logger.Log($"{client.Client.RemoteEndPoint}", "방을 생성하였습니다.", ConsoleColor.DarkYellow);
                byte[] roomNameByte = new byte[DB_GameRoomInfo.ROOM_NAME_SIZE];
                
                MyEncoder.Encode(roomName, roomNameByte, 0, roomNameByte.Length);

                PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_CreateRoomSuccess, roomNameByte);
                dbServer.Send(data.ToPacket(), client);
                EnterRoom(client, roomName);
            }
            else
            {
                Logger.Log($"{client.Client.RemoteEndPoint}", "방 생성에 실패하였습니다.", ConsoleColor.Red);
                PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_CreateRoomFail);
                dbServer.Send(data.ToPacket(), client);
            }
        }
        public void EnterRoom(TcpClient client, string roomName)
        {
            DB_GameRoom gameRoom = DatabaseConnector.GetGameRoom(roomName);
            uint roomId = gameRoom.RoomId;
            DB_RoomUserInfo roomUser = new DB_RoomUserInfo(roomId, dbServer.clients[client].Id, 0);
            DatabaseConnector.TryJoinRoom(roomUser);
        }
        public void EnterRandomRoom(IPEndPoint endPoint)
        {
            Logger.Log($"{endPoint.Address}", "랜덤 방에 참가하였습니다.", ConsoleColor.DarkYellow);

        }
    }
}
