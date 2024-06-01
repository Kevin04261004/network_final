using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using DYUtil;

namespace GameLogicServer
{
    public class LogicServer : BaseServer
    {
        public GameLogicPacketHandler packetHandler;

        public LogicServer(int portNum) : base(portNum)
        {
            packetHandler = new GameLogicPacketHandler();
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
            PacketDataInfo.EPacketType packetType = PacketDataInfo.EPacketType.None;
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
                    packetType = (PacketDataInfo.EPacketType)BitConverter.ToInt16(recvBuffer, offset);
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

                    Debug.Assert(packetType != PacketDataInfo.EPacketType.None);
                    byte[] data = new byte[packetSize - PacketDataInfo.HeaderSize];
                    Array.Copy(partialBuffer, 0, data, 0, data.Length);
                    packetHandler.ProcessPacket(clientIPEndPoint, packetType, data);
                }
            }
        }
        private void InitServer()
        {
            try
            {
                serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 10000);

                serverSock.Bind(endPoint);
            }
            catch (Exception ex)
            {
                Logger.LogError("Binding Error", ex.Message, true);
            }
        }
        private void SetAllHandlers()
        {
            packetHandler.SetHandler(PacketDataInfo.EPacketType.Client_TryConnectToServer, ClientConnected);
            packetHandler.SetHandler(PacketDataInfo.EPacketType.Client_ExitGame, ClientDisConnected);
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
