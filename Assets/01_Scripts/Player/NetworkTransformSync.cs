using System;
using GameLogicServer.Datas.Database;
using UnityEngine;

public class NetworkTransformSync : MonoBehaviour
{
    public NetworkPlayer NetworkPlayerData { get; set; }
    
    private float _time;
    [SerializeField] [Range(4, 10)] private float _sendPacketPerSec = 6;

    private void Update()
    {
        if (!NetworkPlayerData.IsMine)
        {
            return;
        }
        // TODO: Send Current TransformData To Room Clients
        _time += Time.deltaTime;
        if (_time >= ((float)1 / _sendPacketPerSec))
        {
            _time = 0;
            SendPlayerTransformPositionAndRotationPacket();
        }
    }
    private byte[] GetTransformPositionAndRotationPacket()
    {
        byte[] data = new byte[MyVector3Info.GetByteSize() * 2 + DB_UserLoginInfoInfo.NICKNAME_SIZE];
        byte[] posBytes = MyVector3Info.Serialize(transform.position);
        byte[] rotBytes = MyVector3Info.Serialize(transform.rotation);
        byte[] nickNameBytes = new byte[DB_UserLoginInfoInfo.NICKNAME_SIZE];
        
        
        MyEncoder.Encode(NetworkPlayerData.NickName, nickNameBytes, 0, DB_UserLoginInfoInfo.NICKNAME_SIZE);

        int offset = 0;
        Array.Copy(posBytes, 0, data, offset, posBytes.Length);
        offset += posBytes.Length;
        Array.Copy(rotBytes, 0, data, offset, rotBytes.Length);
        offset += rotBytes.Length;
        Array.Copy(nickNameBytes, 0, data, offset, nickNameBytes.Length);
        offset += nickNameBytes.Length;
            
        var packetData =
            new PacketData<PacketDataInfo.EP2PPacketType>(PacketDataInfo.EP2PPacketType.PlayerID_TransformPositionAndRotation, data);
        return packetData.ToPacket();
    }

    private void SendPlayerTransformPositionAndRotationPacket()
    {
        RoomManager.Instance.SendPacket(RoomManager.ESendTo.AllClientsExceptSelf, GetTransformPositionAndRotationPacket());
    }
}
