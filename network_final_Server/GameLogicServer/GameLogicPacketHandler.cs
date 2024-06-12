using DYUtil;
using GameLogicServer.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer
{
    public class GameLogicPacketHandler : PacketHandler<PacketDataInfo.EGameLogicPacketType, IPEndPoint>
    {
        public GameLogicPacketHandler()
        {
            // Enum의 각 값에 대해 델리게이트 추가
            foreach (PacketDataInfo.EGameLogicPacketType packetType in Enum.GetValues(typeof(PacketDataInfo.EGameLogicPacketType)))
            {
                packetHandlerEvents.Add(packetType, null); // 기본값은 null로 초기화
            }
        }
        public override void SetHandler(PacketDataInfo.EGameLogicPacketType packetType, PacketHandlerEvent handler)
        {
            packetHandlerEvents[packetType] = handler;
        }
    }
}
