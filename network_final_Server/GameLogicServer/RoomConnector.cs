using DYUtil;
using GameLogicServer.Datas;
using GameLogicServer.Datas.Database;
using System.Net;
using System.Net.Sockets;

namespace GameLogicServer
{
    public class RoomConnector
    {
        public DBServer dbServer;
        Random random;

        public RoomConnector(DBServer logicServer)
        {
            this.dbServer = logicServer;
            random = new Random();
        }

        public void CreateRoom(TcpClient client, DB_GameRoom gameRoom)
        {
            if (DatabaseConnector.HasRoom(gameRoom.RoomName))
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
            DB_RoomUserInfoInfo.TryParseIPEndPoint((IPEndPoint)client.Client.RemoteEndPoint, out string str);
            DB_RoomUserInfo roomUser = new DB_RoomUserInfo(roomId, dbServer.clients[client].Id, str, false, 0, false);
            if (!DatabaseConnector.HasRoomUser(gameRoom.RoomId))
            {
                roomUser.IsHost = true;
            }
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
        public void ExitRoom(TcpClient client)
        {
            /* 주의: 이 부분은 LogicServer에서 IPEndPoint를 사용하여 삭제한다. */

            //if (!dbServer.clients.ContainsKey(client))
            //{
            //    return;
            //}
            //DatabaseConnector.ExitRoom(dbServer.clients[client].Id);
        }
        public void EnterRandomRoom(TcpClient client)
        {
            Logger.Log($"{client.Client.RemoteEndPoint}", "랜덤 방에 참가하였습니다.", ConsoleColor.DarkYellow);

            if (DatabaseConnector.FindCanJoinRoom(out DB_GameRoom canJoinRoom))
            {
                EnterRoom(client, canJoinRoom);
            }
            else
            {
                int randomNumber = random.Next(1000, 10000);
                DB_GameRoom gameRoom = new DB_GameRoom($"Room{randomNumber}");
                CreateRoom(client, gameRoom);
            }
        }
    }
}
