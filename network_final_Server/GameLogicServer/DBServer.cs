using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net;
using DYUtil;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
