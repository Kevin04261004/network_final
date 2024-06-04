using System.Diagnostics;

namespace GameLogicServer.Datas
{
    public class PacketData
    {
        public short PacketType { get; private set; }
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
                Array.Copy(_data, value, length);
            }
        }
        // 패킷 타입 (2byte) + 패킷 길이(2byte) + 패킷ID(1byte) + Data 길이(N byte);
        public short PacketSize { get => (short)(PacketDataInfo.HeaderSize + (Data == null ? 0 : Data.Length)); }
        public PacketData(PacketDataInfo.EGameLogicPacketType packetType)
        {
            PacketType = (short)packetType;
            PacketID = PacketDataInfo.GetID();
            Data = null;
        }
        public PacketData(PacketDataInfo.EGameLogicPacketType packetType, byte[] data)
        {
            Debug.Assert(data != null);
            PacketID = PacketDataInfo.GetID();
            PacketType = (short)packetType;
            Data = data;
        }
        public byte[] ToPacket()
        {
            byte[] dataSizeBytes = BitConverter.GetBytes(PacketSize);
            byte[] IDBytes = BitConverter.GetBytes(PacketID);
            byte[] typeBytes = BitConverter.GetBytes(PacketType);
            byte[] sendBytes = new byte[dataSizeBytes.Length + IDBytes.Length + typeBytes.Length + (Data == null ? 0 : Data.Length)];

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
}
