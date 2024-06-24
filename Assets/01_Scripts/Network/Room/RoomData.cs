using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    public Dictionary<int, DB_RoomUserInfo> players { get; set; } = new Dictionary<int, DB_RoomUserInfo>();

    public void Awake()
    {
        GameLogicPacketHandler.Instance.SetHandler(PacketDataInfo.EGameLogicPacketType.Server_P2P_ClientEnter, P2P_ClientEnter);
        GameLogicPacketHandler.Instance.SetHandler(PacketDataInfo.EGameLogicPacketType.Server_P2P_ClientExit, P2P_ClientExit);
    }

    /* for Debug */
    [ContextMenu("Print Players Dictionary")]
    public void PrintDictionary()
    {
        foreach (var player in players)
        {
            Debug.Log($"{player.Key}: {player.Value.Id}, {player.Value.IPEndPoint}");
        }
    }
    
    private void P2P_ClientEnter(IPEndPoint endPoint, byte[] data)
    {
        Debug.Assert(data != null);
        int dataSize = DB_RoomUserInfoInfo.GetByteSize();
        int count = data.Length / dataSize; 
        byte[] userData = new byte[dataSize];
        
        for (int i = 0; i < count; ++i)
        {
            Array.Copy(data, i * dataSize, userData, 0, dataSize);
            DB_RoomUserInfo temp = DB_RoomUserInfoInfo.DeSerialize(userData);
            UserEnterRoom(i, temp);
        }
    }

    private void P2P_ClientExit(IPEndPoint endPoint, byte[] data)
    {
        Debug.Assert(data != null);
        int dataSize = DB_RoomUserInfoInfo.GetByteSize();
        byte[] userData = new byte[dataSize];
        Array.Copy(data, 0, userData, 0, dataSize);
        DB_RoomUserInfo temp = DB_RoomUserInfoInfo.DeSerialize(userData);
        UserExitRoom(temp);
    }
    
    private void UserEnterRoom(int gameId, DB_RoomUserInfo userInfo)
    {
        if (!players.TryAdd(gameId, userInfo))
        {
            players[gameId] = userInfo;
        }
    }
    private void UserExitRoom(DB_RoomUserInfo userInfo)
    {
        foreach (var player in players.Where(player => player.Value.Id == userInfo.Id))
        {
            players.Remove(player.Key);
            break;
        }
    }

}
