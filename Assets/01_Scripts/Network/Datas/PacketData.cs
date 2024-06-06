using System;
using System.Diagnostics;

public class PacketData
{
    public Int16 PacketType { get; private set; }
    public char PacketID { get; private set; } // 절대로 0이 될 수 없어야함.
    private byte[]? _data;
    public byte[]? Data
    {
        get => _data;
        set
        {
            if (value == null)
            {
                _data = null;
                return;
            }
            int length = value.Length;
            _data = new byte[length];
            Array.Copy(value, _data, length);
        }
    }
    // 패킷 타입 (2byte) + 패킷 길이(2byte) + 패킷ID(1byte) + Data 길이(N byte);
    public Int16 PacketSize { get => (Int16)(PacketDataInfo.HeaderSize + ((Data == null) ? 0 : Data.Length)); }
    public PacketData(PacketDataInfo.EGameLogicPacketType gameLogicPacketType)
    {
        PacketType = (Int16)gameLogicPacketType;
        PacketID = PacketDataInfo.GetID();
        Data = null;
    }
    public PacketData(PacketDataInfo.EGameLogicPacketType gameLogicPacketType, byte[] data)
    {
        Debug.Assert(data != null);
        PacketID = PacketDataInfo.GetID();
        PacketType = (Int16)gameLogicPacketType;
        Data = data;
    }
    public byte[] ToPacket()
    {
        byte[] dataSizeBytes = BitConverter.GetBytes(PacketSize);
        byte[] IDBytes = BitConverter.GetBytes(PacketID);
        byte[] typeBytes = BitConverter.GetBytes(PacketType);
        byte[] sendBytes = new byte[dataSizeBytes.Length + IDBytes.Length + typeBytes.Length + ((Data == null) ? 0 : Data.Length)];
        
        int offset = 0;
        Array.Copy(dataSizeBytes, 0, sendBytes, offset, dataSizeBytes.Length);
        offset += dataSizeBytes.Length;
        Array.Copy(IDBytes, 0, sendBytes, offset, dataSizeBytes.Length);
        offset += IDBytes.Length;
        Array.Copy(typeBytes, 0, sendBytes, offset, typeBytes.Length);
        if (Data != null)
        {
            offset += typeBytes.Length;
            Array.Copy(Data, 0, sendBytes, offset, Data.Length);
        }

        return sendBytes;
    }
}