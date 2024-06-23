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
        public Dictionary<uint, HashSet<IPEndPoint>> rooms;
        private LogicServer logicServer;
        public RoomHandler(LogicServer server)
        {
            logicServer = server;
            rooms = new Dictionary<uint, HashSet<IPEndPoint>>();
        }

        public void ClientEnterRoom(uint roomId, IPEndPoint endPoint)
        {
            if (!HasRoom(roomId))
            {
                AddRoom(roomId);
            }
            rooms[roomId].Add(endPoint);
        }
        public void ClientExitRoom(uint roomId, IPEndPoint endPoint)
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
            Debug.Assert(HasRoom(roomId));
            foreach(IPEndPoint targetClient in rooms[roomId])
            {
                logicServer.Send(data, targetClient);
            }
        }
        private void AddRoom(uint roomId)
        {
            rooms.Add(roomId, new HashSet<IPEndPoint>());
        }
        private bool HasRoom(uint roomId)
        {
            return rooms.ContainsKey(roomId);
        }
    }
}
