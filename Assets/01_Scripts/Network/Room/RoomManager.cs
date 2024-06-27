using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using GameLogicServer.Datas.Database;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    public NetworkPlayerEnterRoom OnNetworkPlayerEnter { get; set; }
    public NetworkPlayerExitRoom OnNetworkPlayerExit { get; set; }
    public delegate void NetworkPlayerEnterRoom(NetworkPlayer networkPlayer);
    public delegate void NetworkPlayerExitRoom(NetworkPlayer networkPlayer);
    public List<NetworkPlayer> players { get; set; } = new List<NetworkPlayer>();
    [SerializeField] private RoomScene roomScene;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
        foreach (var player in players.Where(player => userInfo.Id.TrimEnd('\0') == player.Id))
        {
            player.roomUserInfo = userInfo;
            return;
        }

        UserEnterRoom(userInfo);
    }
    // TODO: 이곳에서 플레이어 생성 및 삭제 진행.
    private void UserEnterRoom(DB_RoomUserInfo userInfo)
    {
        NetworkPlayer player = new NetworkPlayer(userInfo);
        players.Add(player);
    }
    private void ConnectUserLoginInfo(DB_UserLoginInfo loginInfo)
    {
        foreach (var player in players.Where(player => loginInfo.Id.TrimEnd('\0') == player.Id))
        {
            /* first set info */
            if (!player.IsUserLoginInfoConnected)
            {
                player.userLoginInfo = loginInfo;
                Debug.Assert(OnNetworkPlayerEnter != null);
                OnNetworkPlayerEnter(player);
            }
            /* reset info */
            player.userLoginInfo = loginInfo;
            return;
        }

        Debug.Assert(false);
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

    /* P2P Send */
    public enum ESendTo
    {
        Self,
        AllClients,
        AllClientsExceptSelf,
    }
    public void SendPacket(ESendTo sendTo, byte[] packet)
    {
        switch (sendTo)
        {
            case ESendTo.Self:
                SendPacket(GetMyIPEndPoint(), packet);
                break;
            case ESendTo.AllClients:
                foreach (var player in players)
                {
                    SendPacket(player.IPEndPoint, packet);
                }
                break;
            case ESendTo.AllClientsExceptSelf:
                foreach (var player in players.Where(player => !player.IsMine))
                {
                    SendPacket(player.IPEndPoint, packet);
                }
                break;
            default:
                Debug.Assert(false, "Add Case!!!");
                break;
        }
    }

    public IPEndPoint GetMyIPEndPoint()
    {
        IPEndPoint myIPEndPoint = null;
        foreach (var player in players)
        {
            if (player.IsMine)
            {
                myIPEndPoint = player.IPEndPoint;
            }
        }

        Debug.Assert(myIPEndPoint != null);
        
        return myIPEndPoint;
    }
    
    private void SendPacket(IPEndPoint targetClient, byte[] packet)
    {
        Debug.Assert(NetworkManager.Instance.GameLogicUDPClientSock != null);
        NetworkManager.Instance.GameLogicUDPClientSock.SendTo(packet, targetClient);
    }

    public void ProcessData(IPEndPoint serverIPEndPoint, Int16 packetType, byte[] buffer)
    {
        PacketDataInfo.EP2PPacketType type = (PacketDataInfo.EP2PPacketType)packetType;
        
        RoomPacketHandler.Instance.ProcessPacket(serverIPEndPoint, type, buffer);
    }
}
