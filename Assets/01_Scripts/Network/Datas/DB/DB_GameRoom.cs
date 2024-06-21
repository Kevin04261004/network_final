public class DB_GameRoom
{
    public int RoomId { get; set; }
    public string RoomName { get; set; }
    public char MaxPlayer { get; set; }
    public bool IsPublic { get; set; }
    public string? RoomPassword { get; set; }
    public bool IsPlaying { get; set; }

    public DB_GameRoom(string roomName, char maxPlayer = (char)4, bool isPublic = true, string? roomPW = null, bool isPlaying = false)
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
    public DB_GameRoom(string roomName, int roomId, char maxPlayer = (char)4, bool isPublic = true, string? roomPW = null, bool isPlaying = false)
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