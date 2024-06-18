using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer.Datas.Database
{
    public class DB_UserGameData
    {
        public string Id { get; set; }
        public long SumPoint { get; set; }
        public int MaxPoint { get; set; }
        
        public DB_UserGameData(string id, long sumPoint, int maxPoint)
        {
            Id = id;
            SumPoint = sumPoint;
            MaxPoint = maxPoint;
        }
    }
}
