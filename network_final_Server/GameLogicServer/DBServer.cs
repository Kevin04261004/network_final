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
            packetHandler.SetHandler(PacketDataInfo.EDataBasePacketType.Client_RequireCheckHasID, ClientCheckHasID);
            packetHandler.SetHandler(PacketDataInfo.EDataBasePacketType.Client_CreateAccount, CreateAccount);  
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
                SendClientGameData(client, id);
            }
            else
            {
                ClientLoginFail(client);
            }
        }
        public void ClientCheckHasID(TcpClient client, byte[] data)
        {
            DB_UserLoginInfo info = DB_UserLoginInfoInfo.Deserialize(data);

            string id = info.Id;
            string password = info.Password;

            if (DatabaseConnector.HasUserId(id))
            {
                // UserID가 존재함.
                ClientCantCreateAccount(client);
            }
            else
            {
                // UserID가 존재하지 않음.
                ClientCanCreateAccount(client);
            }
        }
        public void CreateAccount(TcpClient client, byte[] data)
        {
            DB_UserLoginInfo info = DB_UserLoginInfoInfo.Deserialize(data);

            string id = info.Id;

            if (DatabaseConnector.HasUserId(id))
            {
                CreateAccountFail(client);
                return;
            }

            if (DatabaseConnector.TryCreateAccount(info))
            {
                CreateAccountSuccess(client);
            }
            else
            {
                CreateAccountFail(client);
            }
        }
        #endregion

        private void ClientLoginSuccess(TcpClient client, string nickName)
        {
            clients.Add(client, nickName);
            Logger.Log($"{clients[client]}", "님이 로그인에 성공하였습니다.", ConsoleColor.Green);
            byte[] nickNameBytes = Encoding.UTF8.GetBytes(nickName);
            PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_LoginSuccess, nickNameBytes);
            byte[] packet = data.ToPacket();
            Send(packet, client);
        }
        private void SendClientGameData(TcpClient client, string id)
        {
            DB_UserGameData data = DatabaseConnector.GetUserGameData(id);
            Logger.Log($"{clients[client]}", $"Id: {data.Id}, sumPoint: {data.SumPoint}, maxPoint: {data.MaxPoint}");

            byte[] userGameData = DB_UserGameDataInfo.Serialize(data);
            PacketData packetData = new PacketData(PacketDataInfo.EDataBasePacketType.Server_SendUserGameData, userGameData);
            Send(packetData.ToPacket(), client);
        }
        private void ClientLoginFail(TcpClient client)
        {
            PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_LoginFail);
            byte[] packet = data.ToPacket();
            Send(packet, client);
        }
        private void ClientCantCreateAccount(TcpClient client)
        {
            PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_CantCreateAccount);
            byte[] packet = data.ToPacket();
            Send(packet, client);
        }
        private void ClientCanCreateAccount(TcpClient client)
        {
            PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_CanCreateAccount);
            byte[] packet = data.ToPacket();
            Send(packet, client);
        }
        private void CreateAccountFail(TcpClient client)
        {
            PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_CreateAccountFail);
            byte[] packet = data.ToPacket();
            Send(packet, client);
        }
        private void CreateAccountSuccess(TcpClient client)
        {
            PacketData data = new PacketData(PacketDataInfo.EDataBasePacketType.Server_CreateAccountSuccess);
            byte[] packet = data.ToPacket();
            Send(packet, client);
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
