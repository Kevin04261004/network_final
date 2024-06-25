using DYUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer.Datas
{
    public class RoomHandler
    {

        // TODO: 서버가 시작되었을 떄 DB에 있는 모든 Room정보를 가져와 Dicitionary에 할당할 것.
        public Dictionary<uint, List<IPEndPoint>> rooms;
        private LogicServer logicServer;
        public RoomHandler(LogicServer server)
        {
            logicServer = server;
            rooms = new Dictionary<uint, List<IPEndPoint>>();
        }

        public void AddRoom(uint roomId, IPEndPoint endPoint)
        {
            if (!HasRoom(roomId))
            {
                AddRoom(roomId);
            }
            if (!rooms[roomId].Contains(endPoint))
            {
                rooms[roomId].Add(endPoint);
            }
        }
        public void RemoveRome(uint roomId, IPEndPoint endPoint)
        {
            Debug.Assert(HasRoom(roomId));
            rooms[roomId].Remove(endPoint);
            if (rooms[roomId].Count == 0)
            {
                rooms.Remove(roomId);
            }
        }
        public void SendToRoomClients(uint roomId, byte[] data)
        {
            if (!HasRoom(roomId))
            {
                return;
            }
            foreach(IPEndPoint targetClient in rooms[roomId])
            {
                Logger.Log("TargetClients", $"{targetClient}");
                logicServer.Send(data, targetClient);
            }
        }
        private void AddRoom(uint roomId)
        {
            rooms.Add(roomId, new List<IPEndPoint>());
        }
        private bool HasRoom(uint roomId)
        {
            return rooms.ContainsKey(roomId);
        }
    }
}
