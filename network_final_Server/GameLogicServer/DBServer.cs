using System.Net;
using System.Diagnostics;
using GameLogicServer.Datas;
using GameLogicServer.Datas.Database;
using System.Net.Sockets;
using System.Text;
using DYUtil;

namespace GameLogicServer
{
    public class DBServer : TCPListenerServer<PacketDataInfo.EDataBasePacketType>
    {
        private Dictionary<TcpClient, string> clients;

        public DBServer(int port, PacketHandler<PacketDataInfo.EDataBasePacketType, TcpClient> handler) : base(port, handler)
        {
            clients = new Dictionary<TcpClient, string>();

        }

        protected override void ProcessData(TcpClient client, PacketDataInfo.EDataBasePacketType packetType, byte[] buffer)
        {
            Debug.Assert(packetType != PacketDataInfo.EDataBasePacketType.None);
            packetHandler.ProcessPacket(client, packetType, buffer);
        }

        protected override void SetAllHandlers()
        {
            packetHandler.SetHandler(PacketDataInfo.EDataBasePacketType.Client_TryLogin, ClientTryLogin);
        }
        #region Delegate PacketHandle Functions
        public void ClientTryLogin(TcpClient client, byte[] data)
        {
            DB_UserLoginInfo info = DB_UserLoginInfoInfo.Deserialize(data);

            string id = info.Id;
            string password = info.Password;

            if (DatabaseConnector.TryCheckAccountExist(id, password, out var nickName))
            {
                ClientLoginSuccess(client, nickName);
            }
            else
            {
                ClientLoginFail(client);
            }
        }

        #endregion

        private void ClientLoginSuccess(TcpClient client, string nickName)
        {
            clients.Add(client, nickName);
            using (NetworkStream stream = client.GetStream())
            {
                byte[] nickNameBytes = Encoding.UTF8.GetBytes(nickName);
                PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_LoginSuccess, nickNameBytes);
                byte[] packet = data.ToPacket();
                Send(packet, client);
            }
        }

        private void ClientLoginFail(TcpClient client)
        {
            using (NetworkStream stream = client.GetStream())
            {
                PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_LoginFail);
                byte[] packet = data.ToPacket();
                Send(packet, client);
            }
        }

        protected override void Send(byte[] data, HashSet<TcpClient> targetClients)
        {
            foreach (var client in targetClients)
            {
                Send(data, client);
            }
        }

        protected override void Send(byte[] data, TcpClient targetClient)
        {
            try
            {
                if (targetClient == null || !targetClient.Connected)
                {
                    // 클라이언트가 null이거나 연결되지 않았을 경우 처리
                    Logger.LogError("ERROR", "Trying to send data to a null or disconnected client.");
                    return;
                }
                NetworkStream stream = targetClient.GetStream();
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
            catch (SocketException ex)
            {
                Logger.LogError("Send Error", ex.Message);
            }
        }
    }
}
