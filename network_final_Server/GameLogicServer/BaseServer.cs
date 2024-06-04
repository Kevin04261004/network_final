using System.Net;

namespace GameLogicServer
{
    public abstract class BaseServer<T> : IServer where T : Enum
    {
        protected int portNumber;
        protected static readonly int MAX_BUF_SIZE = 4096;
        public HashSet<IPEndPoint> connectedClients { get; protected set; } = new HashSet<IPEndPoint>();
        public PacketHandler<T> packetHandler { get; protected set; }

        private Thread? receiveThread = null;
        private Thread? sendThread = null;
        public BaseServer(int port, PacketHandler<T> handler)
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
        public void StopServer()
        {
            SocketClose();
            receiveThread?.Abort();
            sendThread?.Abort();
        }
    }
}
