using DYUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer
{
    public class GameLogicPacketHandler : PacketHandler
    {
        private Dictionary<PacketDataInfo.EPacketType, PacketHandlerEvent> packetHandlerEvents =
            new Dictionary<PacketDataInfo.EPacketType, PacketHandlerEvent>();

        public GameLogicPacketHandler()
        {
            // Enum의 각 값에 대해 델리게이트 추가
            foreach (PacketDataInfo.EPacketType packetType in Enum.GetValues(typeof(PacketDataInfo.EPacketType)))
            {
                packetHandlerEvents.Add(packetType, null); // 기본값은 null로 초기화
            }
        }
        public void SetHandler(PacketDataInfo.EPacketType packetType, PacketHandlerEvent handler)
        {
            packetHandlerEvents[packetType] = handler;
        }
        public void ProcessPacket(IPEndPoint clientEndPoint, PacketDataInfo.EPacketType type, byte[] packetBuffer)
        {
            Logger.Log($"{clientEndPoint.Address}", $"packetType: {type}", ConsoleColor.Cyan);
            if (packetHandlerEvents[type] == null)
            {
                Logger.LogError("Process Packet", $"No Function for process {type} packet");
            }
            else
            {
                packetHandlerEvents[type](clientEndPoint, packetBuffer);
            }
        }
    }
}
