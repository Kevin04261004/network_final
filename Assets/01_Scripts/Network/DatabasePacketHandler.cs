using System;
using UnityEngine;

public class DatabasePacketHandler : PacketHandler<PacketDataInfo.EDataBasePacketType>
{
    public DatabasePacketHandler()
    {
        foreach (PacketDataInfo.EDataBasePacketType packetType in Enum.GetValues(
                     typeof(PacketDataInfo.EDataBasePacketType)))
        {
            packetHandlerEvents.Add(packetType, null);
        }
    }
    
    public override void SetHandler(PacketDataInfo.EDataBasePacketType packetType, PacketHandlerEvent handler)
    {
        packetHandlerEvents[packetType] = handler;
    }
}
