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
        public DatabasePacketHandler()
        {
            // Enum의 각 값에 대해 델리게이트 추가
            foreach (PacketDataInfo.EDataBasePacketType packetType in Enum.GetValues(typeof(PacketDataInfo.EDataBasePacketType)))
            {
                packetHandlerEvents.Add(packetType, null); // 기본값은 null로 초기화
            }
        }
        public override void SetHandler(PacketDataInfo.EDataBasePacketType packetType, PacketHandlerEvent handler)
        {
            packetHandlerEvents[packetType] = handler;
        }
    }
}
