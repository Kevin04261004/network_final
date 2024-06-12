using DYUtil;
using GameLogicServer.Datas;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace GameLogicServer
{
    public abstract class TCPListenerServer<T> : BaseServer<T, TcpClient> where T : Enum
    {
        protected TcpListener? listener = null;

        protected TCPListenerServer(int port, PacketHandler<T, TcpClient> handler) : base(port, handler)
        {

        }
        protected override void ReceiveThreadFunc()
        {
            InitServer();
            Debug.Assert(listener != null);
            SetAllHandlers();

            EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            Debug.Assert(clientEndPoint != null);

            T packetType = (T)Enum.ToObject(typeof(T), 0);

            byte[] recvBuffer = new byte[MAX_BUF_SIZE];

            while (true)
            {
                using TcpClient client = listener.AcceptTcpClient();

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
                    packetType = (T)Enum.ToObject(typeof(T), BitConverter.ToInt16(recvBuffer, offset));
                }
                while (packetSize > 0)
                {
                    packetSize -= (Int16)stream.Read(recvBuffer, 0, packetSize);
                }

                ProcessData(client, packetType, recvBuffer);
            }
        }

        protected override void SendThreadFunc()
        {
            return;
        }

        protected abstract void ProcessData(TcpClient client, T packetType, byte[] buffer);

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
