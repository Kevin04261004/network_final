using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer.Datas.Database
{
    public class DB_GameRoom
    {
        public uint RoomId { get; set; }
        public string RoomName { get; set; }
        public byte MaxPlayer { get; set; }
        public bool IsPublic { get; set; }
        public string? RoomPassword { get; set; }
        public bool IsPlaying { get; set; }

        public DB_GameRoom()
        {
            RoomId = 0;
            RoomName = string.Empty;
            MaxPlayer = 0;
            IsPublic = false;
            RoomPassword = string.Empty;
            IsPlaying = false;
        }
        public DB_GameRoom(string roomName, byte maxPlayer = 4, bool isPublic = true, string? roomPW = null, bool isPlaying = false)
        {
            if (!isPublic)
            {
                if (roomPW == null)
                {
                    RoomId = 0;
                    RoomName = roomName;
                    MaxPlayer = maxPlayer;
                    IsPublic = true;
                    RoomPassword = null;
                    IsPlaying = false;
                    return;
                }
            }
            RoomId = 0;
            RoomName = roomName;
            MaxPlayer = maxPlayer;
            IsPublic = isPublic;
            RoomPassword = roomPW;
            IsPlaying = false;
        }
        public DB_GameRoom(string roomName, uint roomId, byte maxPlayer = 4, bool isPublic = true, string? roomPW = null, bool isPlaying = false)
        {
            if (!isPublic)
            {
                if (roomPW == null)
                {
                    RoomId = 0;
                    RoomName = roomName;
                    MaxPlayer = maxPlayer;
                    IsPublic = true;
                    RoomPassword = null;
                    IsPlaying = false;
                    return;
                }
            }
            RoomId = roomId;
            RoomName = roomName;
            MaxPlayer = maxPlayer;
            IsPublic = isPublic;
            RoomPassword = roomPW;
            IsPlaying = isPlaying;
        }
    }
}
