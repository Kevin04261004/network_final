using System;
using System.Text;
using UnityEngine;
/*
+--------------+------------------+------+-----+---------+----------------+
| Field        | Type             | Null | Key | Default | Extra          |
+--------------+------------------+------+-----+---------+----------------+
| roomId       | int unsigned     | NO   | PRI | NULL    | auto_increment |
| roomName     | varchar(16)      | YES  |     | NULL    |                |
| maxPlayer    | tinyint unsigned | NO   |     | NULL    |                |
| isPublic     | tinyint(1)       | NO   |     | NULL    |                |
| roomPassword | varchar(16)      | YES  |     | NULL    |                |
| isPlaying    | tinyint(1)       | NO   |     | NULL    |                |
+--------------+------------------+------+-----+---------+----------------+
*/
public static class DB_GameRoomInfo
{
    public static readonly int ROOM_ID_SIZE = sizeof(int);
    public static readonly int MAX_PLAYER_SIZE = sizeof(char);
    public static readonly int ROOM_NAME_SIZE = 16;
    public static readonly int IS_PUBLIC_SIZE = sizeof(bool);
    public static readonly int ROOM_PW_SIZE = 16;
    public static readonly int IS_PLAYING_SIZE = sizeof(bool);

    public static int GetByteSize()
    {
        return ROOM_ID_SIZE + MAX_PLAYER_SIZE + IS_PUBLIC_SIZE + ROOM_PW_SIZE + IS_PLAYING_SIZE;
    }
    public static byte[] Serialize(DB_GameRoom room)
    {
        Debug.Assert(room != null);

        int size = GetByteSize();
        byte[] data = new byte[size];
        byte[] roomIdBytes = BitConverter.GetBytes(room.RoomId);
        byte[] roomNameBytes = new byte[ROOM_NAME_SIZE];
        byte[] maxPlayerBytes = BitConverter.GetBytes(room.MaxPlayer);
        byte[] isPublicBytes = BitConverter.GetBytes(room.IsPublic);
        byte[] roomPWBytes = new byte[ROOM_PW_SIZE];
        byte[] isPlayingBytes = BitConverter.GetBytes(room.IsPlaying);

        MyEncoder.Encode(room.RoomPassword, roomPWBytes, 0, roomPWBytes.Length);
        MyEncoder.Encode(room.RoomName, roomNameBytes, 0, roomNameBytes.Length);

        int offset = 0;
        Array.Copy(roomIdBytes, 0, data, offset, roomIdBytes.Length);
        offset += roomIdBytes.Length;
        Array.Copy(roomNameBytes, 0, data, offset, roomNameBytes.Length);
        offset += roomNameBytes.Length;
        Array.Copy(maxPlayerBytes, 0, data, offset, maxPlayerBytes.Length);
        offset += maxPlayerBytes.Length;
        Array.Copy(isPublicBytes, 0, data, offset, isPublicBytes.Length);
        offset += isPublicBytes.Length;
        Array.Copy(roomPWBytes, 0, data, offset, roomPWBytes.Length);
        offset += roomPWBytes.Length;
        Array.Copy(isPlayingBytes, 0, data, offset, isPlayingBytes.Length);
        offset += isPlayingBytes.Length;

        return data;
    }
    public static DB_GameRoom DeSerialize(byte[] data)
    {
        byte[] roomIdBytes = new byte[ROOM_ID_SIZE];
        byte[] roomNameBytes = new byte[ROOM_NAME_SIZE];
        byte[] maxPlayerBytes = new byte[MAX_PLAYER_SIZE];
        byte[] isPublicBytes = new byte[IS_PUBLIC_SIZE];
        byte[] roomPWBytes = new byte[ROOM_PW_SIZE];
        byte[] isPlayingBytes = new byte[IS_PLAYING_SIZE];

        int offset = 0;
        Array.Copy(data, offset, roomIdBytes, 0, roomIdBytes.Length);
        offset += roomIdBytes.Length;
        Array.Copy(data, offset, roomNameBytes, 0, roomNameBytes.Length);
        offset += roomNameBytes.Length;
        Array.Copy(data, offset, maxPlayerBytes, 0, maxPlayerBytes.Length);
        offset += maxPlayerBytes.Length;
        Array.Copy(data, offset, isPublicBytes, 0, isPublicBytes.Length);
        offset += isPublicBytes.Length;
        Array.Copy(data, offset, roomPWBytes, 0, roomPWBytes.Length);
        offset += roomPWBytes.Length;
        Array.Copy(data, offset, isPlayingBytes, 0, isPlayingBytes.Length);
        offset += isPlayingBytes.Length;

        int roomId = BitConverter.ToInt32(roomIdBytes);
        string roomName = Encoding.UTF8.GetString(roomNameBytes);
        char maxPlayer = BitConverter.ToChar(maxPlayerBytes);
        bool isPublic = BitConverter.ToBoolean(isPublicBytes);
        string roomPW = Encoding.UTF8.GetString(roomPWBytes);
        bool isPlaying = BitConverter.ToBoolean(isPlayingBytes);

        return new DB_GameRoom(roomName, roomId, maxPlayer, isPublic, roomPW, isPlaying);
    }
}