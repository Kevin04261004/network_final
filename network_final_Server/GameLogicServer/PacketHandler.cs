using DYUtil;
using System.Net;

namespace GameLogicServer
{
    public static class PacketHandler
    {
        public delegate void PacketHandlerEvent(IPEndPoint endPoint, byte[] data);

        private static Dictionary<PacketDataInfo.EPacketType, PacketHandlerEvent> packetHandlerEvents =
            new Dictionary<PacketDataInfo.EPacketType, PacketHandlerEvent>();

        static PacketHandler()
        {
            // Enum의 각 값에 대해 델리게이트 추가
            foreach (PacketDataInfo.EPacketType packetType in Enum.GetValues(typeof(PacketDataInfo.EPacketType)))
            {
                packetHandlerEvents.Add(packetType, null); // 기본값은 null로 초기화
            }
            SetAllHandlers();
        }
        public static void SetHandler(PacketDataInfo.EPacketType packetType, PacketHandlerEvent handler)
        {
            packetHandlerEvents[packetType] = handler;
        }

        public static void ProcessPacket(IPEndPoint clientEndPoint, PacketDataInfo.EPacketType type, byte[] packetBuffer)
        {
            Logger.Log($"{clientEndPoint.Address}", $"packetType: {type}", ConsoleColor.Cyan);
            packetHandlerEvents[type](clientEndPoint, packetBuffer);
        }

        private static void SetAllHandlers()
        {
            SetHandler(PacketDataInfo.EPacketType.Client_TryConnectToServer, LogicServer.ClientConnected);
            SetHandler(PacketDataInfo.EPacketType.Client_ExitGame, LogicServer.ClientDisConnected);
        }
    }
}