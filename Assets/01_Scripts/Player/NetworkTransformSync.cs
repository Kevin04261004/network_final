using System;
using GameLogicServer.Datas.Database;
using UnityEngine;

public class NetworkTransformSync : MonoBehaviour
{
    public NetworkPlayer NetworkPlayerData { get; set; }
    public Vector3 targetPosition { get; set; }
    public Quaternion targetRotation { get; set; }
    public float interpolationStartTime { get; set; }
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerStat _playerStat;
    private float SpeedChangeRate = 12f;
    private static readonly int Speed = Animator.StringToHash("speed");
    private float interpolationDuration => (float)1 / (_sendPacketPerSec - 2);

    private float _time;
    [SerializeField] [Range(4, 10)] private int _sendPacketPerSec = 6;
    
    private void Update()
    {
        if (!NetworkPlayerData.IsMine)
        {
            float speed;
            if (Time.time < interpolationStartTime + interpolationDuration)
            {
                float fraction = (Time.time - interpolationStartTime) / interpolationDuration;
                transform.position = Vector3.Lerp(transform.position, targetPosition, fraction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, fraction);
                float distance = Vector3.Distance(transform.position, targetPosition);
                float targetSpeed = distance > 0.008f ? _playerStat.WalkSpeed : 0f;

                // Smoothly adjust speed towards targetSpeed using Mathf.Lerp
                speed = Mathf.Lerp(_animator.GetFloat(Speed) * _playerStat.RunSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
            }
            else
            {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
                speed = 0;
            }
            _animator.SetFloat(Speed, speed / _playerStat.RunSpeed);
            
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
            new PacketData<PacketDataInfo.EP2PPacketType>(PacketDataInfo.EP2PPacketType.PlayerNickName_TransformPositionAndRotation, data);
        return packetData.ToPacket();
    }

    private void SendPlayerTransformPositionAndRotationPacket()
    {
        RoomManager.Instance.SendPacket(RoomManager.ESendTo.AllClientsExceptSelf, GetTransformPositionAndRotationPacket());
    }
}
