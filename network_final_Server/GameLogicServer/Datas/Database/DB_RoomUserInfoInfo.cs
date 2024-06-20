using DYUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer.Datas.Database
{
    /*
    +--------+--------------+------+-----+---------+-------+
    | Field  | Type         | Null | Key | Default | Extra |
    +--------+--------------+------+-----+---------+-------+
    | roomId | int unsigned | NO   | PRI | NULL    |       |
    | id     | varchar(16)  | NO   | PRI | NULL    |       |
    | score  | int          | YES  |     | NULL    |       |
    +--------+--------------+------+-----+---------+-------+
    */
    public class DB_RoomUserInfoInfo
    {
        public static readonly int ROOM_ID_SIZE = DB_GameRoomInfo.ROOM_ID_SIZE;
        public static readonly int ID_SIZE = DB_UserLoginInfoInfo.ID_SIZE;
        public static readonly int SCORE_SIZE = sizeof(int);

        public static int GetByteSize()
        {
            return ROOM_ID_SIZE + ID_SIZE + SCORE_SIZE;
        }
        public static byte[] Serialize(DB_RoomUserInfo room)
        {
            Debug.Assert(room != null);

            int size = GetByteSize();
            byte[] data = new byte[size];
            byte[] roomIdBytes = BitConverter.GetBytes(room.RoomId);
            byte[] idBytes = new byte[ID_SIZE];
            byte[] scoreBytes = BitConverter.GetBytes(room.Score);

            MyEncoder.Encode(room.Id, idBytes, 0, ID_SIZE);

            int offset = 0;
            Array.Copy(roomIdBytes, 0, data, offset, roomIdBytes.Length);
            offset += roomIdBytes.Length;
            Array.Copy(idBytes, 0, data, offset, idBytes.Length);
            offset += idBytes.Length;
            Array.Copy(scoreBytes, 0, data, offset, scoreBytes.Length);
            offset += scoreBytes.Length;

            return data;
        }
        public static DB_RoomUserInfo DeSerialize(byte[] data)
        {
            byte[] roomIdBytes = new byte[ROOM_ID_SIZE];
            byte[] idBytes = new byte[ID_SIZE];
            byte[] scoreBytes = new byte[SCORE_SIZE];

            int offset = 0;
            Array.Copy(data, offset, roomIdBytes, 0, roomIdBytes.Length);
            offset += roomIdBytes.Length;
            Array.Copy(data, offset, idBytes, 0, idBytes.Length);
            offset += idBytes.Length;
            Array.Copy(data, offset, scoreBytes, 0, scoreBytes.Length);
            offset += scoreBytes.Length;

            uint roomId = BitConverter.ToUInt32(roomIdBytes);
            string id = Encoding.UTF8.GetString(idBytes);
            int score = BitConverter.ToInt32(scoreBytes);

            return new DB_RoomUserInfo(roomId, id, score);
        }
    }
}
