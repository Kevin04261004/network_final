using DYUtil;
using GameLogicServer.Datas;
using MySqlX.XDevAPI;
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
                    AsyncAcceptServer().Wait();
                }
                catch (Exception ex)
                {
                    Logger.LogError("ReceiveThreadFunc Error", ex.Message, true);
                }
            }
        }

        private async Task AsyncAcceptServer()
        {
            while(true)
            {
                TcpClient connectedTCPClient = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                Console.WriteLine(connectedTCPClient.Client.RemoteEndPoint.ToString());

                Task.Factory.StartNew(AsyncTCPProcess, connectedTCPClient);
            }
        }

        private async void AsyncTCPProcess(object connectedSock)
        {
            TcpClient tc = (TcpClient)connectedSock;
            NetworkStream stream = tc.GetStream();
            byte[] receiveBuffer = new byte[MAX_BUF_SIZE];
            try
            {
                while (true)
                {
                    if (!tc.Connected)
                    {
                        Console.WriteLine($"연결 끊김");
                        return;
                    }

                    Debug.Assert(stream != null, "STREAM이 NULL입니다.");

                    int byteRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
                    if (byteRead <= 0) 
                    {
                        throw new Exception("Connection closed unexpectedly.");
                    }

                    byte[] headerBuffer = new byte[PacketDataInfo.HeaderSize];
                    byte[] dataBuffer = new byte[byteRead - PacketDataInfo.HeaderSize];

                    Buffer.BlockCopy(receiveBuffer, 0, headerBuffer, 0, headerBuffer.Length);
                    Buffer.BlockCopy(receiveBuffer, PacketDataInfo.HeaderSize, dataBuffer, 0, dataBuffer.Length);

                    int offset = 0;
                    Int16 packetSize = BitConverter.ToInt16(headerBuffer, offset);
                    offset += PacketDataInfo.PacketSizeSize;
                    char packetID = BitConverter.ToChar(headerBuffer, offset);
                    offset += PacketDataInfo.PacketIDSize;
                    T packetType = (T)Enum.ToObject(typeof(T), BitConverter.ToInt16(headerBuffer, offset));

                    ProcessData(tc, packetType, dataBuffer);

                    /*// Read header first
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
                    T packetType = (T)Enum.ToObject(typeof(T), BitConverter.ToInt16(headerBuffer, offset));

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
                    totalBytesRead = 0;
                    ProcessData(tc, packetType, dataBuffer);*/
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                tc.Close();
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
