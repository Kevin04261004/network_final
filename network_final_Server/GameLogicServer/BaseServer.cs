using System.Net.Sockets;
using System.Net;
using DYUtil;

namespace GameLogicServer
{
    public abstract class BaseServer<PacketType> : IServer where PacketType : Enum
    {
        protected int portNumber;
        protected static readonly int MAX_BUF_SIZE = 4096;
        public HashSet<IPEndPoint> connectedClients { get; protected set; } = new HashSet<IPEndPoint>();
        public PacketHandler<PacketType> packetHandler { get; protected set; }

        private Thread? thread = null;
        public BaseServer(int port, PacketHandler<PacketType> handler)
        {
            portNumber = port;
            packetHandler = handler;
        }

        public abstract void ServerFunction();
        protected abstract void SocketClose();
        public void StartServer()
        {
            Thread thread = new Thread(ServerFunction)
            {
                IsBackground = true
            };
            thread.Start();
        }

        public void StopServer()
        {
            SocketClose();
            thread?.Abort();
        }
    }
}
