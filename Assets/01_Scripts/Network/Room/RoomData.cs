using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using GameLogicServer.Datas.Database;
using UnityEngine;

public class NetworkPlayer
{
    public DB_UserLoginInfo userLoginInfo { private get; set; }
    public DB_RoomUserInfo roomUserInfo { private get; set; }
    
    public bool IsMine
    {
        get
        {
            DB_RoomUserInfoInfo.TryParseIPEndPoint((IPEndPoint)NetworkManager.Instance.GameLogicUDPClientSock.RemoteEndPoint, out string str);
            return str == roomUserInfo.IPEndPoint;
        }
    }
    public bool IsHost => roomUserInfo.IsHost;
    public IPEndPoint IPEndPoint
    {
        get
        {
            DB_RoomUserInfoInfo.TryParseIPEndPoint(roomUserInfo.IPEndPoint, out IPEndPoint endPoint);
            return endPoint;
        }
    }
    public string NickName => userLoginInfo.NickName.TrimEnd('\0');
    public string Id => roomUserInfo.Id.TrimEnd('\0');
    public uint OrderInRoom => roomUserInfo.OrderinRoom;
    public bool IsReady => roomUserInfo.IsReady;
    
    public NetworkPlayer(DB_RoomUserInfo roomUserInfo)
    {
        this.roomUserInfo = roomUserInfo;
        this.userLoginInfo = null;
    }
};
public class RoomData : MonoBehaviour
{
    private NetworkPlayerEnterRoom OnNetworkPlayerEnter;
    private NetworkPlayerExitRoom OnNetworkPlayerExit;
    public delegate void NetworkPlayerEnterRoom(NetworkPlayer networkPlayer);
    public delegate void NetworkPlayerExitRoom(NetworkPlayer networkPlayer);
    public List<NetworkPlayer> players { get; set; } = new List<NetworkPlayer>();
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
            Debug.Log($"{player.OrderInRoom}: EndPoint_{player.IPEndPoint}, Id_{player.Id}");
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
        List<NetworkPlayer> newList = players.OrderBy(t => t.OrderInRoom).ToList();
        players.Clear();
        players = newList;
    }
    private void UpdateOrAddRoomUserInfo(DB_RoomUserInfo userInfo)
    {
        foreach (var player in players)
        {
            if (userInfo.Id.TrimEnd('\0') == player.Id)
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
            if (loginInfo.Id.TrimEnd('\0') == player.Id)
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
        NetworkPlayer player = new NetworkPlayer(userInfo);
        players.Add(player);

        Debug.Assert(OnNetworkPlayerEnter != null);
        OnNetworkPlayerEnter(player);
    }
    private void UserExitRoom(uint exitPlayerOrderRoomId)
    {
        foreach (var player in players.Where(player => player.OrderInRoom == exitPlayerOrderRoomId))
        {
            players.Remove(player);
            
            Debug.Assert(OnNetworkPlayerExit != null);
            OnNetworkPlayerExit(player);
            break;
        }
    }
}
