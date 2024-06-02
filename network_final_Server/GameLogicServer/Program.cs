using DYUtil;

namespace GameLogicServer
{
    public class Program
    {
        public static readonly int GAME_LOGIC_PORT_NUM = 10000;
        public static readonly int DATABASE_PORT_NUM = 10001;

        static void Main(string[] args)
        {
            PacketHandler<PacketDataInfo.EGameLogicPacketType> gameLogicPacketHandler = new GameLogicPacketHandler();
            BaseServer<PacketDataInfo.EGameLogicPacketType> gameLogicServer = new LogicServer(GAME_LOGIC_PORT_NUM, gameLogicPacketHandler);

            PacketHandler<PacketDataInfo.EDataBasePacketType> dataBasePacketHandler = new DatabasePacketHandler();
            BaseServer<PacketDataInfo.EDataBasePacketType> dataBaseServer = new DBServer(DATABASE_PORT_NUM, dataBasePacketHandler);

            IPFinder.TryGetMyIPv4(out string myIp);

            gameLogicServer.StartServer();
            Logger.Log($"{myIp}:{GAME_LOGIC_PORT_NUM}", "Game Server Started...", ConsoleColor.Green);
            dataBaseServer.StartServer();
            Logger.Log($"{myIp}:{DATABASE_PORT_NUM}", "DB Server Started...", ConsoleColor.Green);

            Logger.Log("Info", "Enter To Exit Server", ConsoleColor.Blue);
            Console.ReadLine();
            
            gameLogicServer.StopServer();
            Logger.Log($"{myIp}:{GAME_LOGIC_PORT_NUM}", "Game Server Closed...", ConsoleColor.Red);
            dataBaseServer.StopServer();
            Logger.Log($"{myIp}:{DATABASE_PORT_NUM}", "DB Server Closed...", ConsoleColor.Red);
        
            Console.ResetColor();
        }
    }
}
