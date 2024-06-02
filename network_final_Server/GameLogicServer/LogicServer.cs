using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using DYUtil;

namespace GameLogicServer
{
    public class LogicServer : BaseServer<PacketDataInfo.EGameLogicPacketType>
    {
        protected Socket? serverSock = null;
        public LogicServer(int portNum, PacketHandler<PacketDataInfo.EGameLogicPacketType> handler) : base(portNum, handler)
        {
        }

        public override void ServerFunction()
        {
            InitServer();
            Debug.Assert(serverSock != null);
            SetAllHandlers();

            EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            Debug.Assert(clientEndPoint != null);

            byte[] recvBuffer = new byte[MAX_BUF_SIZE];
            byte[] partialBuffer = new byte[MAX_BUF_SIZE];
            int partialSize = 0;

            Int16 packetSize = 0;
            PacketDataInfo.EGameLogicPacketType packetType = PacketDataInfo.EGameLogicPacketType.None;
            while (true)
            {
                int recvByteSize = serverSock.ReceiveFrom(recvBuffer, ref clientEndPoint);

                IPEndPoint clientIPEndPoint = (IPEndPoint)clientEndPoint;
                if (partialSize == 0)
                {
                    if (recvByteSize < PacketDataInfo.HeaderSize)
                    {
                        continue;
                    }
                    int offset = 0;
                    packetSize = BitConverter.ToInt16(recvBuffer, offset);
                    offset += PacketDataInfo.PacketSizeSize;
                    char packetID = BitConverter.ToChar(recvBuffer, offset);
                    offset += PacketDataInfo.PacketIDSize;
                    packetType = (PacketDataInfo.EGameLogicPacketType)BitConverter.ToInt16(recvBuffer, offset);
                    offset += PacketDataInfo.PacketTypeSize;
                    Array.Copy(recvBuffer, offset, partialBuffer, partialSize, recvByteSize);
                    partialSize = recvByteSize;
                }
                else
                {
                    Array.Copy(recvBuffer, 0, partialBuffer, partialSize, recvByteSize);
                    partialSize += recvByteSize;
                }
                if (partialSize >= packetSize)
                {
                    partialSize = 0;

                    Debug.Assert(packetType != PacketDataInfo.EGameLogicPacketType.None);
                    byte[] data = new byte[packetSize - PacketDataInfo.HeaderSize];
                    Array.Copy(partialBuffer, 0, data, 0, data.Length);
                    packetHandler.ProcessPacket(clientIPEndPoint, packetType, data);
                }
            }
        }

        protected override void SocketClose()
        {
            serverSock?.Close();
        }

        private void InitServer()
        {
            try
            {
                serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, portNumber);

                serverSock.Bind(endPoint);
            }
            catch (Exception ex)
            {
                Logger.LogError("Binding Error", ex.Message, true);
            }
        }
        private void SetAllHandlers()
        {
            packetHandler.SetHandler(PacketDataInfo.EGameLogicPacketType.Client_TryConnectToServer, ClientConnected);
            packetHandler.SetHandler(PacketDataInfo.EGameLogicPacketType.Client_ExitGame, ClientDisConnected);
        }

        #region Delegate PacketHandle Functions
        public void ClientConnected(IPEndPoint endPoint, byte[] data)
        {
            Logger.Log($"{endPoint.Address}", "Success Connect Server", ConsoleColor.Green);

            connectedClients.Add(endPoint);
        }
        public void ClientDisConnected(IPEndPoint endPoint, byte[] data)
        {
            Logger.Log($"{endPoint.Address}", "DisConnected", ConsoleColor.DarkMagenta);

            connectedClients.Remove(endPoint);
        }

        #endregion
    }
}
