using System.Net;
using UnityEngine;

public class DatabaseClient : TCPClient<PacketDataInfo.EDataBasePacketType>
{
    protected override void ProcessData(IPEndPoint serverIPEndPoint, PacketDataInfo.EDataBasePacketType packetType, byte[] buffer)
    {
        Debug.Assert(packetType != PacketDataInfo.EDataBasePacketType.None);

        DatabasePacketHandler.Instance.ProcessPacket(serverIPEndPoint, packetType, buffer);
    }

    protected override void OnApplicationQuit()
    {
        CloseServer();
    }
}
