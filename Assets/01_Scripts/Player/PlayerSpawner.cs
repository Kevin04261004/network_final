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
        
        RoomPacketHandler.Instance.SetHandler(PacketDataInfo.EP2PPacketType.PlayerNickName_TransformPositionAndRotation, SetNetworkPlayerTransform);
        RoomPacketHandler.Instance.SetHandler(PacketDataInfo.EP2PPacketType.PlayerNickName_KeyInput, SetAnimationWithKeyInput);
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
            GameObject spawnPlayer = Instantiate(Player_Prefab, transform.localPosition, transform.rotation, transform);
            NetworkPlayerHandler handler = spawnPlayer.GetComponent<NetworkPlayerHandler>();
            SpawnedPlayer.Add(networkPlayer.NickName.TrimEnd('\0'), handler);
            handler.NetworkPlayerData = networkPlayer;
        });
    }
    private void DeletePlayer(NetworkPlayer networkPlayer)
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            Destroy(SpawnedPlayer[networkPlayer.NickName.TrimEnd('\0')].gameObject);
            SpawnedPlayer.Remove(networkPlayer.NickName.TrimEnd('\0'));
        });
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
    private void SetAnimationWithKeyInput(IPEndPoint endPoint, byte[] data)
    {
        byte[] wasd_Bytes = new byte[sizeof(bool)];
        byte[] shift_Bytes = new byte[sizeof(bool)];
        byte[] nickNameBytes = new byte[DB_UserLoginInfoInfo.NICKNAME_SIZE];

        int offset = 0;
        Array.Copy(data, offset, wasd_Bytes, 0, wasd_Bytes.Length);
        offset += wasd_Bytes.Length;
        Array.Copy(data, offset, shift_Bytes, 0, shift_Bytes.Length);
        offset += shift_Bytes.Length;
        Array.Copy(data, offset, nickNameBytes, 0, nickNameBytes.Length);
        offset += nickNameBytes.Length;

        bool wasd = BitConverter.ToBoolean(wasd_Bytes);
        bool shift = BitConverter.ToBoolean(shift_Bytes);
        string nickName = Encoding.UTF8.GetString(nickNameBytes);
        SpawnedPlayer[nickName.TrimEnd('\0')].SetAnimation(wasd, shift);
    }
    #endregion
}
