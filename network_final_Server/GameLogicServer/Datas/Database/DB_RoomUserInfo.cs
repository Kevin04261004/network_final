using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer.Datas.Database
{
    public class DB_RoomUserInfo
    {
        public uint RoomId { get; set; }
        public string Id { get; set; }
        public int Score { get; set; }

        public DB_RoomUserInfo(uint roomId, string id, int score)
        {
            RoomId = roomId;
            Id = id;
            Score = score;
        }
        public DB_RoomUserInfo()
        {
            RoomId = 0;
            Id = string.Empty;
            Score = 0;
        }
    }
}
