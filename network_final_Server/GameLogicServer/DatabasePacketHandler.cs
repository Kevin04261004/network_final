using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GameLogicServer.Datas;

namespace GameLogicServer
{
    public class DatabasePacketHandler : PacketHandler<PacketDataInfo.EDataBasePacketType>
    {
        public override void SetHandler(PacketDataInfo.EDataBasePacketType packetType, PacketHandlerEvent handler)
        {

        }
    }
}
