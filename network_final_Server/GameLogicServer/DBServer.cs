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
    public class DBServer : BaseServer<PacketDataInfo.EDataBasePacketType>
    {
        TcpListener? listener = null;
        public DBServer(int port, PacketHandler<PacketDataInfo.EDataBasePacketType> handler) : base(port, handler)
        {

        }

        public override void ServerFunction()
        {
            InitServer();
            Debug.Assert(listener != null);
            SetAllHandlers();

            EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            Debug.Assert(clientEndPoint != null);

            PacketDataInfo.EDataBasePacketType packetType = PacketDataInfo.EDataBasePacketType.None;

            byte[] recvBuffer = new byte[MAX_BUF_SIZE];

            while (true)
            {
                using TcpClient client = listener.AcceptTcpClient();
                IPEndPoint clientIPEndPoint = (IPEndPoint)clientEndPoint;

                NetworkStream stream = client.GetStream();

                Int16 packetSize = 0;
                char packetID;

                int offset = 0;

                if (stream.Length < PacketDataInfo.HeaderSize)
                {
                    continue;
                }
                if (stream.Read(recvBuffer, 0, PacketDataInfo.PacketSizeSize) == PacketDataInfo.PacketSizeSize)
                {
                    offset = 0;
                    packetSize = (Int16)((int)BitConverter.ToInt16(recvBuffer, offset) - PacketDataInfo.HeaderSize); // 여기서 패킷 사이즈를 Data만으로 자름.
                    offset += PacketDataInfo.PacketSizeSize;
                    packetID = BitConverter.ToChar(recvBuffer, offset);
                    offset += PacketDataInfo.PacketIDSize;
                    packetType = (PacketDataInfo.EDataBasePacketType)BitConverter.ToInt16(recvBuffer, offset);
                }
                while (packetSize > 0)
                {
                    packetSize -= (Int16)stream.Read(recvBuffer, 0, packetSize);
                }
                // 패킷의 사이즈만큼 다 읽음.

                Debug.Assert(packetType != PacketDataInfo.EDataBasePacketType.None);
                packetHandler.ProcessPacket(clientIPEndPoint, packetType, recvBuffer);
            }
        }

        protected override void SocketClose()
        {
            listener?.Stop();
        }

        private void InitServer()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, portNumber);
                listener.Start(10);
            }
            catch (Exception ex)
            {
                Logger.LogError("Binding Error", ex.Message, true);
            }
        }
        private void SetAllHandlers()
        {
            
        }
    }
}
