using System.Net.Sockets;
using System.Net;

namespace GameLogicServer
{
    public abstract class BaseServer
    {
        protected int portNumber;
        protected static readonly int MAX_BUF_SIZE = 1024;
        protected Socket? serverSock = null;
        public HashSet<IPEndPoint> connectedClients = new HashSet<IPEndPoint>();

        public BaseServer(int port)
        {
            portNumber = port;
        }

        public abstract void ServerFunction();
    }
}
