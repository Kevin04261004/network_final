using System.Net;
using System.Threading.Tasks;
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
        Task.Run(async () =>
        {
            await SendExitMessageAndCloseServer();
        }).Wait();
    }

    private async Task SendExitMessageAndCloseServer()
    {
        var packetData =
            new PacketData<PacketDataInfo.EDataBasePacketType>(PacketDataInfo.EDataBasePacketType.Client_ExitGameDB);
        await NetworkManager.Instance.SendToServerAsync(ESendServerType.Database, packetData.ToPacket());
        CloseServer();
    }
}
