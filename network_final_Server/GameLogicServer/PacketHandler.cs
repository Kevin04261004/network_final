using DYUtil;
using System.Diagnostics;
using System.Net;

namespace GameLogicServer
{
    public abstract class PacketHandler
    {
        public delegate void PacketHandlerEvent(IPEndPoint endPoint, byte[] data);
    }
}