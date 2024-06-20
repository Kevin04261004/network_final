using DYUtil;
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
        public void CreateRoom(IPEndPoint endPoint)
        {
            DB_GameRoom room = new DB_GameRoom();
            if (DatabaseConnector.TryCraeteRoom(room))
            {

            }
            {
                Logger.Log($"{endPoint.Address}", "방을 생성하였습니다.", ConsoleColor.DarkYellow);

            }
        }
        public void EnterRandomRoom(IPEndPoint endPoint)
        {
            Logger.Log($"{endPoint.Address}", "랜덤 방에 참가하였습니다.", ConsoleColor.DarkYellow);

        }
    }
}
