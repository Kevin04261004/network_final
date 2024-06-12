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

            T packetType = (T)Enum.ToObject(typeof(T), 0);

            while (true)
            {
                try
                {
                    using TcpClient client = listener.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();

                    byte[] headerBuffer = new byte[PacketDataInfo.HeaderSize];
                    int totalBytesRead = 0;

                    // Read header first
                    while (totalBytesRead < headerBuffer.Length)
                    {
                        int bytesRead = stream.Read(headerBuffer, totalBytesRead, headerBuffer.Length - totalBytesRead);
                        if (bytesRead == 0)
                        {
                            throw new Exception("Connection closed unexpectedly.");
                        }
                        totalBytesRead += bytesRead;
                    }

                    int offset = 0;
                    Int16 packetSize = BitConverter.ToInt16(headerBuffer, offset);
                    offset += PacketDataInfo.PacketSizeSize;
                    char packetID = BitConverter.ToChar(headerBuffer, offset);
                    offset += PacketDataInfo.PacketIDSize;
                    packetType = (T)Enum.ToObject(typeof(T), BitConverter.ToInt16(headerBuffer, offset));

                    // Read data part
                    byte[] dataBuffer = new byte[packetSize - PacketDataInfo.HeaderSize];
                    totalBytesRead = 0;

                    while (totalBytesRead < dataBuffer.Length)
                    {
                        int bytesRead = stream.Read(dataBuffer, totalBytesRead, dataBuffer.Length - totalBytesRead);
                        if (bytesRead == 0)
                        {
                            throw new Exception("Connection closed unexpectedly.");
                        }
                        totalBytesRead += bytesRead;
                    }

                    ProcessData(client, packetType, dataBuffer);
                }
                catch (Exception ex)
                {
                    Logger.LogError("ReceiveThreadFunc Error", ex.Message, true);
                }
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
