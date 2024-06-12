using DYUtil;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace GameLogicServer
{
    public abstract class SocketUDPServer<PacketType> : BaseServer<PacketType, IPEndPoint> where PacketType : Enum
    {
        protected Socket? serverSock = null;

        protected SocketUDPServer(int port, PacketHandler<PacketType, IPEndPoint> handler) : base(port, handler)
        {
        }

        protected override void ReceiveThreadFunc()
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
            PacketType packetType = (PacketType)Enum.ToObject(typeof(PacketType), 0);
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
                    packetType = (PacketType)Enum.ToObject(typeof(PacketType), BitConverter.ToInt16(recvBuffer, offset));
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

                    byte[] data = new byte[packetSize - PacketDataInfo.HeaderSize];
                    Array.Copy(partialBuffer, 0, data, 0, data.Length);
                    ProcessData(clientIPEndPoint, packetType, data);
                }
            }
        }

        protected override void SendThreadFunc()
        {

        }

        protected override void Send(byte[] data, HashSet<IPEndPoint> targetClients)
        {
            Debug.Assert(serverSock != null);
            foreach (var client in targetClients)
            {
                Send(data, client);
            }
        }
        protected override void Send(byte[] data, IPEndPoint targetClient)
        {
            Debug.Assert(serverSock != null);
            serverSock.SendTo(data, targetClient);
        }

        protected abstract void ProcessData(IPEndPoint clientIPEndPoint, PacketType packetType, byte[] buffer);

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
        protected abstract void SetAllHandlers();
        protected override void SocketClose()
        {
            serverSock?.Close();
        }
    }
}
