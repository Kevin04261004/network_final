using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject Player_Prefab;
    private Dictionary<NetworkPlayer, NetworkPlayerHandler> SpawnedPlayer = new Dictionary<NetworkPlayer, NetworkPlayerHandler>();
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
            SpawnedPlayer.Add(networkPlayer, handler); 
        });
    }
    private void DeletePlayer(NetworkPlayer networkPlayer)
    {
        Destroy(SpawnedPlayer[networkPlayer].gameObject);
        SpawnedPlayer.Remove(networkPlayer);
    }

    #region Delegate Functions
    private void SetNetworkPlayerTransform(IPEndPoint endPoint, byte[] data)
    {
        byte[] posBytes = new byte[MyVector3Info.GetByteSize()];
        byte[] rotBytes = new byte[MyVector3Info.GetByteSize()];
        byte[] roomOrderBytes = new byte[DB_RoomUserInfoInfo.ORDER_IN_ROOM_SIZE];

        int offset = 0;
        Array.Copy(data, offset, posBytes, 0, posBytes.Length);
        offset += posBytes.Length;
        Array.Copy(data, offset, rotBytes, 0, rotBytes.Length);
        offset += rotBytes.Length;
        Array.Copy(data, offset, roomOrderBytes, 0, roomOrderBytes.Length);
        offset += roomOrderBytes.Length;

        Vector3 pos = MyVector3Info.ToVector3(posBytes);
        Quaternion rot = MyVector3Info.ToQuaternion(rotBytes);
        uint roomOrderNum = BitConverter.ToUInt32(roomOrderBytes);
        FindNetworkPlayer(roomOrderNum).SetPosAndRot(pos, rot);
    }
    #endregion

    private NetworkPlayerHandler FindNetworkPlayer(uint orderRoomNum)
    {
        foreach (var p in SpawnedPlayer.Keys.Where(p => p.OrderInRoom == orderRoomNum))
        {
            return SpawnedPlayer[p];
        }
        Debug.Assert(false, "Network Player Can not Found");
        return null;
    }
}
