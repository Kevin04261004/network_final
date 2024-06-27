using System;

public class RoomPacketHandler : PacketHandler<PacketDataInfo.EP2PPacketType>
{
    public RoomPacketHandler()
    {
        foreach (PacketDataInfo.EP2PPacketType packetType in Enum.GetValues(typeof(PacketDataInfo.EP2PPacketType)))
        {
            packetHandlerEvents.Add(packetType, null);
        }
    }
    public override void SetHandler(PacketDataInfo.EP2PPacketType packetType, PacketHandlerEvent handler)
    {
        packetHandlerEvents[packetType] = handler;
    }
}