using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using GameLogicServer.Datas.Database;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject Player_Prefab;
    private Dictionary<string, NetworkPlayerHandler> SpawnedPlayer = new Dictionary<string, NetworkPlayerHandler>();
    private void Awake()
    {
        RoomManager.Instance.OnNetworkPlayerEnter += SpawnPlayer;
        RoomManager.Instance.OnNetworkPlayerExit += DeletePlayer;
        
        RoomPacketHandler.Instance.SetHandler(PacketDataInfo.EP2PPacketType.PlayerID_TransformPositionAndRotation, SetNetworkPlayerTransform);
    }
    private void OnDisable()
    {
        RoomManager.Instance.OnNetworkPlayerEnter -= SpawnPlayer;
        RoomManager.Instance.OnNetworkPlayerExit -= DeletePlayer;
    }
    private void SpawnPlayer(NetworkPlayer networkPlayer)
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            GameObject spawnPlayer = Instantiate(Player_Prefab, transform.position, transform.rotation, transform);
            NetworkPlayerHandler handler = spawnPlayer.GetComponent<NetworkPlayerHandler>();
            handler.NetworkPlayerData = networkPlayer;
            SpawnedPlayer.Add(networkPlayer.NickName.TrimEnd('\0'), handler); 
        });
    }
    private void DeletePlayer(NetworkPlayer networkPlayer)
    {
        Destroy(SpawnedPlayer[networkPlayer.NickName.TrimEnd('\0')].gameObject);
        SpawnedPlayer.Remove(networkPlayer.NickName.TrimEnd('\0'));
    }

    #region Delegate Functions
    private void SetNetworkPlayerTransform(IPEndPoint endPoint, byte[] data)
    {
        byte[] posBytes = new byte[MyVector3Info.GetByteSize()];
        byte[] rotBytes = new byte[MyVector3Info.GetByteSize()];
        byte[] nickNameBytes = new byte[DB_UserLoginInfoInfo.NICKNAME_SIZE];

        int offset = 0;
        Array.Copy(data, offset, posBytes, 0, posBytes.Length);
        offset += posBytes.Length;
        Array.Copy(data, offset, rotBytes, 0, rotBytes.Length);
        offset += rotBytes.Length;
        Array.Copy(data, offset, nickNameBytes, 0, nickNameBytes.Length);
        offset += nickNameBytes.Length;

        Vector3 pos = MyVector3Info.ToVector3(posBytes);
        Quaternion rot = MyVector3Info.ToQuaternion(rotBytes);
        string nickName = Encoding.UTF8.GetString(nickNameBytes);
        SpawnedPlayer[nickName.TrimEnd('\0')].SetPosAndRot(pos, rot);
    }
    #endregion
}
