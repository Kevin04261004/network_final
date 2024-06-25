﻿using System;
using System.Net;
using System.Text;
using GameLogicServer.Datas.Database;
using UnityEngine;

/*
+-------------+--------------+------+-----+---------+-------+
| Field       | Type         | Null | Key | Default | Extra |
+-------------+--------------+------+-----+---------+-------+
| roomId      | int unsigned | NO   | PRI | NULL    |       |
| id          | varchar(16)  | NO   | PRI | NULL    |       |
| IPEndPoint  | varchar(32)  | NO   |     | NULL    |       |
| isReady     | tinyint(1)   | NO   |     | 0       |       |
| orderInRoom | int unsigned | NO   |     | 0       |       |
| isHost      | tinyint(1)   | NO   |     | 0       |       |
+-------------+--------------+------+-----+---------+-------+
*/
public class DB_RoomUserInfoInfo
{
    public static readonly int ROOM_ID_SIZE = DB_GameRoomInfo.ROOM_ID_SIZE;
    public static readonly int ID_SIZE = DB_UserLoginInfoInfo.ID_SIZE;
    public static readonly int IPENDPOINT_SIZE = 32;
    public static readonly int IS_READY_SIZE = sizeof(bool);
    public static readonly int ORDER_IN_ROOM_SIZE = sizeof(uint);
    public static readonly int IS_HOST_SIZE = sizeof(bool);
    public static int GetByteSize()
    {
        return ROOM_ID_SIZE + ID_SIZE + IPENDPOINT_SIZE + IS_READY_SIZE + ORDER_IN_ROOM_SIZE + IS_HOST_SIZE;
    }
    public static byte[] Serialize(DB_RoomUserInfo room)
    {
        Debug.Assert(room != null);

        int size = GetByteSize();
        byte[] data = new byte[size];
        byte[] roomIdBytes = BitConverter.GetBytes(room.RoomId);
        byte[] idBytes = new byte[ID_SIZE];
        byte[] ipEndPointBytes = new byte[IPENDPOINT_SIZE];
        byte[] isReadyBytes = BitConverter.GetBytes(room.IsReady);
        byte[] orderInRoomBytes = BitConverter.GetBytes(room.OrderinRoom);
        byte[] isHostBytes = BitConverter.GetBytes(room.IsHost);

        MyEncoder.Encode(room.Id, idBytes, 0, ID_SIZE);
        MyEncoder.Encode(room.IPEndPoint, ipEndPointBytes, 0, IPENDPOINT_SIZE);
        int offset = 0;
        Array.Copy(roomIdBytes, 0, data, offset, roomIdBytes.Length);
        offset += roomIdBytes.Length;
        Array.Copy(idBytes, 0, data, offset, idBytes.Length);
        offset += idBytes.Length;
        Array.Copy(ipEndPointBytes, 0, data, offset, ipEndPointBytes.Length);
        offset += ipEndPointBytes.Length;
        Array.Copy(isReadyBytes, 0, data, offset, isReadyBytes.Length);
        offset += isReadyBytes.Length;
        Array.Copy(orderInRoomBytes, 0, data, offset, orderInRoomBytes.Length);
        offset += orderInRoomBytes.Length;
        Array.Copy(isHostBytes, 0, data, offset, isHostBytes.Length);
        offset += isHostBytes.Length;

        return data;
    }
    public static DB_RoomUserInfo DeSerialize(byte[] data)
    {
        byte[] roomIdBytes = new byte[ROOM_ID_SIZE];
        byte[] idBytes = new byte[ID_SIZE];
        byte[] ipEndPointBytes = new byte[IPENDPOINT_SIZE];
        byte[] isReadyBytes = new byte[IS_READY_SIZE];
        byte[] orderInRoomBytes = new byte[ORDER_IN_ROOM_SIZE];
        byte[] isHostBytes = new byte[IS_HOST_SIZE];

        int offset = 0;
        Array.Copy(data, offset, roomIdBytes, 0, roomIdBytes.Length);
        offset += roomIdBytes.Length;
        Array.Copy(data, offset, idBytes, 0, idBytes.Length);
        offset += idBytes.Length;
        Array.Copy(data, offset, ipEndPointBytes, 0, ipEndPointBytes.Length);
        offset += ipEndPointBytes.Length;
        Array.Copy(data, offset, isReadyBytes, 0, isReadyBytes.Length);
        offset += isReadyBytes.Length;
        Array.Copy(data, offset, orderInRoomBytes, 0, orderInRoomBytes.Length);
        offset += orderInRoomBytes.Length;
        Array.Copy(data, offset, isHostBytes, 0, isHostBytes.Length);
        offset += isHostBytes.Length;

        uint roomId = BitConverter.ToUInt32(roomIdBytes);
        string id = Encoding.UTF8.GetString(idBytes);
        string ipEndPoint = Encoding.UTF8.GetString(ipEndPointBytes);
        bool isReady = BitConverter.ToBoolean(isReadyBytes);
        uint orderInRoom = BitConverter.ToUInt32(orderInRoomBytes);
        bool isHost = BitConverter.ToBoolean(isHostBytes);

        return new DB_RoomUserInfo(roomId, id, ipEndPoint, isReady, orderInRoom, isHost);
    }
    public static bool TryParseIPEndPoint(string str, out IPEndPoint endPoint)
    {
        endPoint = null;

        if (string.IsNullOrWhiteSpace(str))
        {
            return false;
        }

        string[] parts = str.Split(':');
        if (parts.Length != 2)
        {
            return false;
        }

        if (!IPAddress.TryParse(parts[0], out IPAddress ipAddress))
        {
            return false;
        }

        if (!int.TryParse(parts[1], out int port))
        {
            return false;
        }

        endPoint = new IPEndPoint(ipAddress, port);
        return true;
    }
    public static bool TryParseIPEndPoint(IPEndPoint endPoint, out string str)
    {
        str = null;

        if (endPoint == null)
        {
            return false;
        }

        str = $"{endPoint.Address}:{endPoint.Port}";
        return true;
    }
}