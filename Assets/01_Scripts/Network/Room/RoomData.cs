using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using GameLogicServer.Datas.Database;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    public Dictionary<int, DB_RoomUserInfo> players { get; set; } = new Dictionary<int, DB_RoomUserInfo>();
    [SerializeField] private RoomScene roomScene;
    public Dictionary<DB_RoomUserInfo, DB_UserLoginInfo> userInfos { get; set; } =
        new Dictionary<DB_RoomUserInfo, DB_UserLoginInfo>();
    public void Awake()
    {
        GameLogicPacketHandler.Instance.SetHandler(PacketDataInfo.EGameLogicPacketType.Server_P2P_ClientEnter, P2P_ClientEnter);
        GameLogicPacketHandler.Instance.SetHandler(PacketDataInfo.EGameLogicPacketType.Server_P2P_ClientExit, P2P_ClientExit);
        GameLogicPacketHandler.Instance.SetHandler(PacketDataInfo.EGameLogicPacketType.Server_P2P_ClientUserLoginInfo, P2P_ClientUserLoginInfo);
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
        DisConnectUserLoginInfo(temp);
    }

    private void P2P_ClientUserLoginInfo(IPEndPoint endPoint, byte[] data)
    {
        Debug.Assert(data != null);
        int dataSize = DB_UserLoginInfoInfo.GetByteSize();
        int count = data.Length / dataSize; 
        byte[] userData = new byte[dataSize];
        
        for (int i = 0; i < count; ++i)
        {
            Array.Copy(data, i * dataSize, userData, 0, dataSize);
            DB_UserLoginInfo temp = DB_UserLoginInfoInfo.Deserialize(userData);
            ConnectUserLoginInfo(i, temp);
        }
        roomScene.SetPanel(userInfos);
    }
    private void UserEnterRoom(int gameId, DB_RoomUserInfo userInfo)
    {
        if (!players.TryAdd(gameId, userInfo))
        {
            players[gameId] = userInfo;
        }
    }

    private void ConnectUserLoginInfo(int index, DB_UserLoginInfo loginInfo)
    {
        if (!userInfos.TryAdd(players[index], loginInfo))
        {
            userInfos[players[index]] = loginInfo;
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
    private void DisConnectUserLoginInfo(DB_RoomUserInfo roomUserInfo)
    {
        userInfos.Remove(roomUserInfo);
    }
}
