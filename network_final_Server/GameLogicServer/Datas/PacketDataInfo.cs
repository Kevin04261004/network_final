namespace GameLogicServer
{
    public static class PacketDataInfo
    {
        public enum EGameLogicPacketType
        {
            None = 0,
            Client_TryConnectToServer,
            Client_ExitGame,
            Client_RequireCreateNetworkObject,
            Server_CreateNetworkObjectSuccess,
            Client_EnterRandomRoom,
            Client_CreateRoom,
            Server_CreateRoomFail,
            Server_CreateRoomSuccess,

        }
        public enum EDataBasePacketType
        {
            None = 0,
            Client_TryLogin,
            Server_LoginFail,
            Server_LoginSuccess,
            Server_SendUserGameData,
            Client_RequireCheckHasID,
            Server_CanCreateAccount,
            Server_CantCreateAccount,
            Client_CreateAccount,
            Server_CreateAccountFail,
            Server_CreateAccountSuccess,
        }

        public static int PacketIDSize = sizeof(char);
        public static int PacketTypeSize = sizeof(Int16);
        public static int PacketSizeSize = sizeof(Int16);
        public static int HeaderSize { get => PacketSizeSize + PacketIDSize + PacketTypeSize; }

        private static char s_packetID = (char)0;
        public static char GetID()
        {
            s_packetID++;
            if (s_packetID == 0)
            {
                s_packetID++;
            }
            return s_packetID;
        }
    }
}