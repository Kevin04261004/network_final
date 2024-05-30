using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using DYUtil;

namespace GameLogicServer
{
    public static class LogicServer
    {
        public static readonly int PORT_NUM = 10000;
        private static readonly int MAX_BUF_SIZE = 1024;
        private static readonly int CHECK_BEFORE_ID_SIZE = 10;
        private static Socket? serverSock = null;
        public static HashSet<IPEndPoint> connectedClients = new HashSet<IPEndPoint>();

        public static void ServerFunction()
        {
            InitServer();
            Debug.Assert(serverSock != null);
            SetAllHandlers();

            EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, PORT_NUM);
            Debug.Assert(clientEndPoint != null);

            byte[] recvBuffer = new byte[MAX_BUF_SIZE];
            byte[] partialBuffer = new byte[MAX_BUF_SIZE];
            int partialSize = 0;

            Queue<char> beforeID = new Queue<char>(5);
            char packetID = (char)0;
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
                    packetID = BitConverter.ToChar(recvBuffer, offset);
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
                    if(!beforeID.Contains(packetID))
                    {
                        Debug.Assert(packetType != PacketDataInfo.EPacketType.None);
                        byte[] data = new byte[packetSize - PacketDataInfo.HeaderSize];
                        Array.Copy(partialBuffer, 0, data, 0, data.Length);
                        PacketHandler.ProcessPacket(clientIPEndPoint, packetType, data);
                    }
                    if(beforeID.Count > CHECK_BEFORE_ID_SIZE)
                    {
                        beforeID.Dequeue();
                    }
                    beforeID.Enqueue(packetID);
                }
            }
        }
        private static void InitServer()
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
        private static void SetAllHandlers()
        {
            PacketHandler.SetHandler(PacketDataInfo.EPacketType.Client_TryConnectToServer, LogicServer.ClientConnected);
            PacketHandler.SetHandler(PacketDataInfo.EPacketType.Client_ExitGame, LogicServer.ClientDisConnected);
        }

        #region Delegate PacketHandle Functions
        public static void ClientConnected(IPEndPoint endPoint, byte[] data)
        {
            Logger.Log($"{endPoint.Address}", "Success Connect Server", ConsoleColor.Green);

            connectedClients.Add(endPoint);
        }
        public static void ClientDisConnected(IPEndPoint endPoint, byte[] data)
        {
            Logger.Log($"{endPoint.Address}", "DisConnected", ConsoleColor.Red);

            connectedClients.Remove(endPoint);
        }
        #endregion
    }
}
