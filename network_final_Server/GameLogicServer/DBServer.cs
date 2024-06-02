using System;
using System.Net;
using DYUtil;
using System.Diagnostics;

namespace GameLogicServer
{
    public class DBServer : TCPListenerServer<PacketDataInfo.EDataBasePacketType>
    {
        public DBServer(int port, PacketHandler<PacketDataInfo.EDataBasePacketType> handler) : base(port, handler)
        {

        }

        protected override void ProcessData(IPEndPoint clientIPEndPoint, PacketDataInfo.EDataBasePacketType packetType, byte[] buffer)
        {
            Debug.Assert(packetType == PacketDataInfo.EDataBasePacketType.None);
            packetHandler.ProcessPacket(clientIPEndPoint, packetType, buffer);
        }

        protected override void SetAllHandlers()
        {
            
        }
    }
}
