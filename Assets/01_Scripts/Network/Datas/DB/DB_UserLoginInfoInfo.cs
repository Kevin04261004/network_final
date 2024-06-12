using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameLogicServer.NetworkObjectDataInfo;

namespace GameLogicServer.Datas.Database
{
    /*
     * network_final_multi - UserLoginInfo Table Desc
     * 
    +----------+-------------+------+-----+---------+-------+
    | Field    | Type        | Null | Key | Default | Extra |
    +----------+-------------+------+-----+---------+-------+
    | nickName | varchar(16) | NO   | PRI | NULL    |       |
    | id       | varchar(16) | NO   |     | NULL    |       |
    | password | varchar(16) | NO   |     | NULL    |       |
    +----------+-------------+------+-----+---------+-------+
    */
    public static class DB_UserLoginInfoInfo
    {
        public static readonly int NICKNAME_SIZE = 16;
        public static readonly int ID_SIZE = 16;
        public static readonly int PASSWORD_SIZE = 16;
        public static int GetByteSize()
        {
            int size = NICKNAME_SIZE + ID_SIZE + PASSWORD_SIZE;

            return size;
        }

        public static byte[] Serialize(DB_UserLoginInfo userLoginInfo)
        {
            Debug.Assert(userLoginInfo != null);

            int size = GetByteSize();
            byte[] data = new byte[size];

            int offset = 0;
            Encoding.UTF8.GetBytes(userLoginInfo.NickName, 0, Math.Min(NICKNAME_SIZE, userLoginInfo.NickName.Length), data, offset);
            offset += NICKNAME_SIZE;
            Encoding.UTF8.GetBytes(userLoginInfo.Id, 0, Math.Min(ID_SIZE, userLoginInfo.Id.Length), data, offset);
            offset += ID_SIZE;
            Encoding.UTF8.GetBytes(userLoginInfo.Password, 0, Math.Min(PASSWORD_SIZE, userLoginInfo.Password.Length), data, offset);
            offset += PASSWORD_SIZE;

            return data;
        }

        public static DB_UserLoginInfo Deserialize(byte[] data)
        {
            Debug.Assert(data != null);

            byte[] nickNameBytes = new byte[NICKNAME_SIZE];
            byte[] idBytes = new byte[ID_SIZE];
            byte[] passwordBytes = new byte[PASSWORD_SIZE];

            int offset = 0;
            Array.Copy(data, offset, nickNameBytes, 0, NICKNAME_SIZE);
            offset += NICKNAME_SIZE;
            Array.Copy(data, offset, idBytes, 0, ID_SIZE);
            offset += ID_SIZE;
            Array.Copy(data, offset, passwordBytes, 0, PASSWORD_SIZE);
            offset += PASSWORD_SIZE;

            string nickName = Encoding.UTF8.GetString(nickNameBytes);
            string id = Encoding.UTF8.GetString(idBytes);
            string password = Encoding.UTF8.GetString(passwordBytes);

            return new DB_UserLoginInfo(nickName, id, password);
        }
    }
}
