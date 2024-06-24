using DYUtil;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace GameLogicServer
{
    public abstract class PacketHandler<T, R>
    {
        protected Dictionary<T, PacketHandlerEvent> packetHandlerEvents = new Dictionary<T, PacketHandlerEvent>();
        public delegate void PacketHandlerEvent(R receiveFrom, byte[] data);

        public abstract void SetHandler(T packetType, PacketHandlerEvent handler);
        public void ProcessPacket(R receiveFrom, T type, byte[] packetBuffer)
        {
            Logger.Log($"{GetAddress(receiveFrom)}", $"packetType: {type}", ConsoleColor.Cyan);
            if (packetHandlerEvents[type] == null)
            {
                Logger.LogError("Process Packet", $"No Function for process {type} packet");
            }
            else
            {
                packetHandlerEvents[type](receiveFrom, packetBuffer);
            }
        }
        public string GetAddress(R receiveFrom)
        {
            switch(receiveFrom)
            {
                case IPEndPoint endPoint:
                    return endPoint.ToString();
                case TcpClient tcpClient:
                    if (tcpClient.Client.RemoteEndPoint is IPEndPoint clientEndPoint)
                    {
                        return clientEndPoint.ToString();
                    }
                    break;
                default:
                    Debug.Assert(false, "Add Case");
                    break;
            }
            return "Unknown";
        }
    }
}