using DYUtil;
using GameLogicServer.Datas;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace GameLogicServer
{
    public abstract class TCPListenerServer<PacketType> : BaseServer<PacketType> where PacketType : Enum
    {
        TcpListener? listener = null;

        protected TCPListenerServer(int port, PacketHandler<PacketType> handler) : base(port, handler)
        {

        }
        protected override void ReceiveThreadFunc()
        {
            InitServer();
            Debug.Assert(listener != null);
            SetAllHandlers();

            EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            Debug.Assert(clientEndPoint != null);

            PacketType packetType = (PacketType)Enum.ToObject(typeof(PacketType), 0);

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
                    packetType = (PacketType)Enum.ToObject(typeof(PacketType), BitConverter.ToInt16(recvBuffer, offset));
                }
                while (packetSize > 0)
                {
                    packetSize -= (Int16)stream.Read(recvBuffer, 0, packetSize);
                }

                ProcessData(clientIPEndPoint, packetType, recvBuffer);
            }
        }

        protected override void SendThreadFunc()
        {
            return;
        }

        protected abstract void ProcessData(IPEndPoint clientIPEndPoint, PacketType packetType, byte[] buffer);

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
        protected abstract void SetAllHandlers();
        protected override void SocketClose()
        {
            listener?.Stop();
        }
    }
}
