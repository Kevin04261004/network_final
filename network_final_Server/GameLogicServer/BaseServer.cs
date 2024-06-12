using System.Diagnostics;
using System.Net;

namespace GameLogicServer
{
    public abstract class BaseServer<T, R> : IServer where T : Enum
    {
        protected int portNumber;
        protected static readonly int MAX_BUF_SIZE = 4096;
        public HashSet<IPEndPoint> connectedClients { get; protected set; } = new HashSet<IPEndPoint>();
        public PacketHandler<T, R> packetHandler { get; protected set; }

        private Thread? receiveThread = null;
        private Thread? sendThread = null;
        public BaseServer(int port, PacketHandler<T, R> handler)
        {
            portNumber = port;
            packetHandler = handler;
        }

        protected abstract void ReceiveThreadFunc();
        protected abstract void SendThreadFunc();
        protected abstract void SocketClose();
        public void StartServer()
        {
            receiveThread = new Thread(ReceiveThreadFunc)
            {
                IsBackground = true
            };

            sendThread = new Thread(SendThreadFunc)
            {
                IsBackground = true
            };

            receiveThread.Start();
            sendThread.Start();
        }
        protected abstract void Send(byte[] data, HashSet<R> targetClients);
        protected abstract void Send(byte[] data, R targetClient);
        public void StopServer()
        {
            SocketClose();
            receiveThread?.Abort();
            sendThread?.Abort();
        }
    }
}
