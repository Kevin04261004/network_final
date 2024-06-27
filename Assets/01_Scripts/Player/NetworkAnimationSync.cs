using System;
using GameLogicServer.Datas.Database;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class NetworkAnimationSync : MonoBehaviour
{
    public NetworkPlayer NetworkPlayerData { get; set; }
    [SerializeField] private Animator _animator;
    private static readonly int Speed = Animator.StringToHash("speed");
    [SerializeField] private PlayerStat _playerStat;

    public bool MoveKeyDown { get; set; } = false;
    public bool ShiftKeyDown { get; set; } = false;
    
    private float _time;
    [SerializeField] [Range(4, 10)] private int _sendPacketPerSec = 6;
    private void Update()
    {
        if (!NetworkPlayerData.IsMine)
        {
            /* Set Animation Value */
            // float moveSpeed = !MoveKeyDown ? 0 : (ShiftKeyDown ? _playerStat.RunSpeed : _playerStat.WalkSpeed);
            // _animator.SetFloat(Speed, moveSpeed / _playerStat.RunSpeed);
            
            return;
        }

        _time += Time.deltaTime;
        if (_time >= ((float)1 / _sendPacketPerSec))
        {
            _time = 0;
            // SendInputPacket();
        }
    }
    // private byte[] GetInputPacket()
    // {
    //     /* 굳이 bit flag를 써야할까 */
    //     byte[] data = new byte[sizeof(bool) * 2 + DB_UserLoginInfoInfo.NICKNAME_SIZE];
    //     byte[] wasd_Bytes = BitConverter.GetBytes(_inputHandler.moveVec != Vector2.zero);
    //     byte[] shift_Bytes = BitConverter.GetBytes(_inputHandler.sprint);
    //     byte[] nickNameBytes = new byte[DB_UserLoginInfoInfo.NICKNAME_SIZE];
    //     
    //     MyEncoder.Encode(NetworkPlayerData.NickName, nickNameBytes, 0, DB_UserLoginInfoInfo.NICKNAME_SIZE);
    //     
    //     int offset = 0;
    //     Array.Copy(wasd_Bytes, 0, data, offset, wasd_Bytes.Length);
    //     offset += wasd_Bytes.Length;
    //     Array.Copy(shift_Bytes, 0, data, offset, shift_Bytes.Length);
    //     offset += shift_Bytes.Length;
    //     Array.Copy(nickNameBytes, 0, data, offset, nickNameBytes.Length);
    //     offset += nickNameBytes.Length;
    //     
    //     var packetData =
    //         new PacketData<PacketDataInfo.EP2PPacketType>(PacketDataInfo.EP2PPacketType.PlayerNickName_KeyInput, data);
    //     return packetData.ToPacket();
    // }
    //
    // private void SendInputPacket()
    // {
    //     RoomManager.Instance.SendPacket(RoomManager.ESendTo.AllClientsExceptSelf, GetInputPacket());
    // }
}
