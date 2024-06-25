using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using GameLogicServer.Datas.Database;
using UnityEngine;

public class RoomUserData
{
    public DB_UserLoginInfo userLoginInfo { get; set; }
    public DB_RoomUserInfo roomUserInfo { get; set; }

    public RoomUserData(DB_RoomUserInfo roomUserInfo)
    {
        this.roomUserInfo = roomUserInfo;
        this.userLoginInfo = null;
    }
};
public class RoomData : MonoBehaviour
{
    public List<RoomUserData> players { get; set; } = new List<RoomUserData>();
    [SerializeField] private RoomScene roomScene;
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
            string endPoint = player.roomUserInfo.IPEndPoint.TrimEnd('\0');
            string id = player.roomUserInfo.Id.TrimEnd('\0');
            Debug.Log($"{player.roomUserInfo.OrderinRoom}: EndPoint_{endPoint}, Id_{id}");
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
            UpdateOrAddRoomUserInfo(temp);
        }
    }

    private void P2P_ClientExit(IPEndPoint endPoint, byte[] data) // data = roomUserBytes + exitPlayerOrderRoomId;
    {
        Debug.Assert(data != null);
        byte[] roomUserBytes = new byte[data.Length - sizeof(uint)];
        byte[] exitPlayerOrderRoomId = new byte[sizeof(uint)];

        int offset = 0;
        Array.Copy(data, offset, roomUserBytes, 0, roomUserBytes.Length);
        offset += roomUserBytes.Length;
        Array.Copy(data, offset, exitPlayerOrderRoomId, 0, exitPlayerOrderRoomId.Length);
        offset += exitPlayerOrderRoomId.Length;

        uint ExitPlayerOrderRoomId = BitConverter.ToUInt32(exitPlayerOrderRoomId);
        UserExitRoom(ExitPlayerOrderRoomId);
        P2P_ClientEnter(endPoint, roomUserBytes);
        
        OrderPlayerListByOrderInRoom();
        roomScene.SetPanel(players);
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
            ConnectUserLoginInfo(temp);
        }
        
        OrderPlayerListByOrderInRoom();
        roomScene.SetPanel(players);
    }

    private void OrderPlayerListByOrderInRoom()
    {
        List<RoomUserData> newList = players.OrderBy(t => t.roomUserInfo.OrderinRoom).ToList();
        players.Clear();
        players = newList;
    }
    private void UpdateOrAddRoomUserInfo(DB_RoomUserInfo userInfo)
    {
        foreach (var player in players)
        {
            if (userInfo.Id == player.roomUserInfo.Id)
            {
                player.roomUserInfo = userInfo;
                return;
            }
        }
        UserEnterRoom(userInfo);
    }
    private void ConnectUserLoginInfo(DB_UserLoginInfo loginInfo)
    {
        foreach (var player in players)
        {
            if (loginInfo.Id == player.roomUserInfo.Id)
            {
                player.userLoginInfo = loginInfo;
                return;
            }
        }
        Debug.Assert(false);
    }

    // TODO: 이곳에서 플레이어 생성 및 삭제 진행.
    private void UserEnterRoom(DB_RoomUserInfo userInfo)
    {
        players.Add(new RoomUserData(userInfo));
    }
    private void UserExitRoom(uint exitPlayerOrderRoomId)
    {
        foreach (var player in players.Where(player => player.roomUserInfo.OrderinRoom == exitPlayerOrderRoomId))
        {
            players.Remove(player);
            break;
        }
    }
}
