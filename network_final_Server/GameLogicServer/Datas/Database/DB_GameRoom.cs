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
        public int RoomId { get; set; }
        public char MaxPlayer { get; set; }
        public bool IsPublic { get; set; }
        public string? RoomPassword { get; set; }
        public bool IsPlaying { get; set; }

        public DB_GameRoom(char maxPlayer = (char)4, bool isPublic = true, string? roomPW = null, bool isPlaying = false)
        {
            if (!isPublic)
            {
                if (roomPW == null)
                {
                    RoomId = 0;
                    MaxPlayer = maxPlayer;
                    IsPublic = true;
                    RoomPassword = null;
                    IsPlaying = false;
                    return;
                }
            }
            RoomId = 0;
            MaxPlayer = maxPlayer;
            IsPublic = isPublic;
            RoomPassword = roomPW;
            IsPlaying = false;
        }
        public DB_GameRoom(int roomId, char maxPlayer = (char)4, bool isPublic = true, string? roomPW = null, bool isPlaying = false)
        {
            if (!isPublic)
            {
                if (roomPW == null)
                {
                    RoomId = 0;
                    MaxPlayer = maxPlayer;
                    IsPublic = true;
                    RoomPassword = null;
                    IsPlaying = false;
                    return;
                }
            }
            RoomId = roomId;
            MaxPlayer = maxPlayer;
            IsPublic = isPublic;
            RoomPassword = roomPW;
            IsPlaying = isPlaying;
        }
    }
}
