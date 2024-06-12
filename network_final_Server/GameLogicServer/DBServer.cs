using System.Net;
using System.Diagnostics;
using GameLogicServer.Datas;
using GameLogicServer.Datas.Database;
using System.Net.Sockets;
using System.Text;

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
            Debug.Assert(packetType == PacketDataInfo.EDataBasePacketType.None);
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
            client.
            using (NetworkStream stream = client.GetStream())
            {
                byte[] nickNameBytes = Encoding.UTF8.GetBytes(nickName);
                PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_LoginSuccess, nickNameBytes);
                byte[] packet = data.ToPacket();
                stream.Write(packet, 0, packet.Length);
            }
        }

        private void ClientLoginFail(TcpClient client)
        {
            using (NetworkStream stream = client.GetStream())
            {
                PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_LoginFail);
                byte[] packet = data.ToPacket();
                stream.Write(packet, 0, packet.Length);
            }
        }
    }
}
