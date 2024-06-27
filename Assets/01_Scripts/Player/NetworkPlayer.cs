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
            var localEndPoint = NetworkManager.Instance.GameLogicUDPClientSock.LocalEndPoint as IPEndPoint;
            if (localEndPoint == null)
            {
                return false;
            }

            if (IPEndPoint.Address.Equals(IPAddress.Parse("127.0.0.1")))
            {
                if (localEndPoint.Port == IPEndPoint.Port)
                {
                    return true;
                }
            }
        
            return IPEndPoint.Address.Equals(localEndPoint.Address) && IPEndPoint.Port == localEndPoint.Port;
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
    public bool IsUserLoginInfoConnected => this.userLoginInfo != null;
    public NetworkPlayer(DB_RoomUserInfo roomUserInfo)
    {
        this.roomUserInfo = roomUserInfo;
        this.userLoginInfo = null;
    }
};