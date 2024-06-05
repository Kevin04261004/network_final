

using System;

public class GameLogicPacketHandler : PacketHandler<PacketDataInfo.EGameLogicPacketType>
{
    public GameLogicPacketHandler()
    {
        foreach (PacketDataInfo.EGameLogicPacketType packetType in Enum.GetValues(
                     typeof(PacketDataInfo.EGameLogicPacketType)))
        {
            packetHandlerEvents.Add(packetType, null);
        }
    }
    public override void SetHandler(PacketDataInfo.EGameLogicPacketType packetType, PacketHandlerEvent handler)
    {
        packetHandlerEvents[packetType] = handler;
    }
}
