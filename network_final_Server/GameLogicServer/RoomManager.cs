using DYUtil;
using GameLogicServer.Datas;
using GameLogicServer.Datas.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer
{
    public class RoomManager
    {
        public LogicServer logicServer;

        public RoomManager(LogicServer logicServer)
        {
            this.logicServer = logicServer;
        }

        public void CreateRoom(IPEndPoint endPoint, string roomName)
        {
            if (DatabaseConnector.HasRoomName(roomName))
            {
                Logger.Log($"{endPoint.Address}", "방 생성에 실패하였습니다.", ConsoleColor.Red);
                PacketData data = new PacketData(PacketDataInfo.EGameLogicPacketType.Server_CreateRoomFail);
                logicServer.Send(data.ToPacket(), endPoint);
                return;
            }
            DB_GameRoom room = new DB_GameRoom(roomName);
            if (DatabaseConnector.TryCraeteRoom(room))
            {
                Logger.Log($"{endPoint.Address}", "방을 생성하였습니다.", ConsoleColor.DarkYellow);
                byte[] roomNameByte = new byte[DB_GameRoomInfo.ROOM_NAME_SIZE];
                
                MyEncoder.Encode(roomName, roomNameByte, 0, roomNameByte.Length);

                PacketData data = new PacketData(PacketDataInfo.EGameLogicPacketType.Server_CreateRoomSuccess, roomNameByte);
                logicServer.Send(data.ToPacket(), endPoint);
                EnterRoom(roomName);
            }
            else
            {
                Logger.Log($"{endPoint.Address}", "방 생성에 실패하였습니다.", ConsoleColor.Red);
                PacketData data = new PacketData(PacketDataInfo.EGameLogicPacketType.Server_CreateRoomFail);
                logicServer.Send(data.ToPacket(), endPoint);
            }
        }
        public void EnterRoom(string roomName)
        {
            DB_GameRoom gameRoom = DatabaseConnector.GetGameRoom(roomName);
            uint roomId = gameRoom.RoomId;
            
        }
        public void EnterRandomRoom(IPEndPoint endPoint)
        {
            Logger.Log($"{endPoint.Address}", "랜덤 방에 참가하였습니다.", ConsoleColor.DarkYellow);

        }
    }
}
