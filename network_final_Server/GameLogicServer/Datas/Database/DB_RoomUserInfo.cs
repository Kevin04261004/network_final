using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer.Datas.Database
{
    public class DB_RoomUserInfo
    {
        public uint RoomId { get; set; }
        public string Id { get; set; }
        public string IPEndPoint { get; set; }
        public bool IsReady { get; set; }
        public uint OrderinRoom { get; set; }
        public bool IsHost { get; set; }
        public DB_RoomUserInfo(uint roomId, string id, string endPoint, bool isReady, uint orderinRoom, bool isHost)
        {
            RoomId = roomId;
            Id = id;
            IPEndPoint = endPoint;
            IsReady = isReady;
            OrderinRoom = orderinRoom;
            IsHost = isHost;
        }
        public DB_RoomUserInfo()
        {
            RoomId = 0;
            Id = string.Empty;
            IPEndPoint = "127.0.0.1:12345";
            IsReady = false;    
            OrderinRoom = 0;
            IsHost = false;
        }
    }
}
