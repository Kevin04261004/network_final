using DYUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer.Datas.Database
{
    public static class DB_UserGameDataInfo
    {
        public static readonly int ID_SIZE = 16;
        public static readonly int SUM_POINT_SIZE = sizeof(long);
        public static readonly int MAX_POINT_SIZE = sizeof(int);
        
        public static int GetByteSize()
        {
            int size = ID_SIZE + SUM_POINT_SIZE + MAX_POINT_SIZE;

            return size;
        }
        public static byte[] Serialize(DB_UserGameData userGameData)
        {
            Debug.Assert(userGameData != null);

            int size = GetByteSize();
            byte[] data = new byte[size];

            int offset = 0;
            MyEncoder.Encode(userGameData.Id, data, offset, ID_SIZE);
            offset += ID_SIZE;
            MyEncoder.Encode(userGameData.SumPoint, data, offset);
            offset += SUM_POINT_SIZE;
            MyEncoder.Encode(userGameData.MaxPoint, data, offset);
            offset += MAX_POINT_SIZE;

            return data;
        }
        public static DB_UserGameData Deserialize(byte[] data)
        {
            Debug.Assert(data != null);

            byte[] idBytes = new byte[ID_SIZE];
            byte[] sumPointBytes = new byte[SUM_POINT_SIZE];
            byte[] maxPointBytes = new byte[MAX_POINT_SIZE];

            int offset = 0;
            Array.Copy(data, offset, idBytes, 0, ID_SIZE);
            offset += ID_SIZE;
            Array.Copy(data, offset, sumPointBytes, 0, SUM_POINT_SIZE);
            offset += SUM_POINT_SIZE;
            Array.Copy(data, offset, maxPointBytes, 0, MAX_POINT_SIZE);
            offset += MAX_POINT_SIZE;

            string id = Encoding.UTF8.GetString(idBytes);
            long sumPoint = BitConverter.ToInt64(sumPointBytes, 0);
            int maxPoint = BitConverter.ToInt32(maxPointBytes, 0);

            return new DB_UserGameData(id, sumPoint, maxPoint);
        }
    }
}
