using DYUtil;
using System.Net;

namespace GameLogicServer
{
    public abstract class PacketHandler<T>
    {
        protected Dictionary<T, PacketHandlerEvent> packetHandlerEvents = new Dictionary<T, PacketHandlerEvent>();
        public delegate void PacketHandlerEvent(IPEndPoint endPoint, byte[] data);

        public abstract void SetHandler(T packetType, PacketHandlerEvent handler);
        public void ProcessPacket(IPEndPoint clientEndPoint, T type, byte[] packetBuffer)
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