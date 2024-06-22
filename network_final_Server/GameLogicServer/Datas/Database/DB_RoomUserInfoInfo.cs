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
    +------------+--------------+------+-----+---------+-------+
    | Field      | Type         | Null | Key | Default | Extra |
    +------------+--------------+------+-----+---------+-------+
    | roomId     | int unsigned | NO   | PRI | NULL    |       |
    | id         | varchar(16)  | NO   | PRI | NULL    |       |
    | IPEndPoint | varchar(32)  | NO   |     | NULL    |       |
    +------------+--------------+------+-----+---------+-------+
    */
    public class DB_RoomUserInfoInfo
    {
        public static readonly int ROOM_ID_SIZE = DB_GameRoomInfo.ROOM_ID_SIZE;
        public static readonly int ID_SIZE = DB_UserLoginInfoInfo.ID_SIZE;
        public static readonly int IPENDPOINT_SIZE = 32;

        public static int GetByteSize()
        {
            return ROOM_ID_SIZE + ID_SIZE + IPENDPOINT_SIZE;
        }
        public static byte[] Serialize(DB_RoomUserInfo room)
        {
            Debug.Assert(room != null);

            int size = GetByteSize();
            byte[] data = new byte[size];
            byte[] roomIdBytes = BitConverter.GetBytes(room.RoomId);
            byte[] idBytes = new byte[ID_SIZE];
            byte[] ipEndPointBytes = new byte[IPENDPOINT_SIZE];

            MyEncoder.Encode(room.Id, idBytes, 0, ID_SIZE);
            MyEncoder.Encode(room.IPEndPoint, ipEndPointBytes, 0, IPENDPOINT_SIZE);
            int offset = 0;
            Array.Copy(roomIdBytes, 0, data, offset, roomIdBytes.Length);
            offset += roomIdBytes.Length;
            Array.Copy(idBytes, 0, data, offset, idBytes.Length);
            offset += idBytes.Length;
            Array.Copy(ipEndPointBytes, 0, data, offset, ipEndPointBytes.Length);
            offset += ipEndPointBytes.Length;

            return data;
        }
        public static DB_RoomUserInfo DeSerialize(byte[] data)
        {
            byte[] roomIdBytes = new byte[ROOM_ID_SIZE];
            byte[] idBytes = new byte[ID_SIZE];
            byte[] ipEndPointBytes = new byte[IPENDPOINT_SIZE];

            int offset = 0;
            Array.Copy(data, offset, roomIdBytes, 0, roomIdBytes.Length);
            offset += roomIdBytes.Length;
            Array.Copy(data, offset, idBytes, 0, idBytes.Length);
            offset += idBytes.Length;
            Array.Copy(data, offset, ipEndPointBytes, 0, ipEndPointBytes.Length);
            offset += ipEndPointBytes.Length;

            uint roomId = BitConverter.ToUInt32(roomIdBytes);
            string id = Encoding.UTF8.GetString(idBytes);
            string ipEndPoint = Encoding.UTF8.GetString(ipEndPointBytes);

            return new DB_RoomUserInfo(roomId, id, ipEndPoint);
        }
    }
}
