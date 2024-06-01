using DYUtil;

namespace GameLogicServer
{
    public class Program
    {
        public static readonly int GAME_LOGIC_PORT_NUM = 10000;

        static void Main(string[] args)
        {
            BaseServer server = new LogicServer(GAME_LOGIC_PORT_NUM);
            Thread serverThread = new Thread(server.ServerFunction)
            {
                IsBackground = true
            };
            serverThread.Start();

            IPFinder.TryGetMyIPv4(out string myIp);
            Logger.Log($"{myIp}:", "Game Server Started...", ConsoleColor.Green);
            Logger.Log("Info", "Enter To Exit Server", ConsoleColor.Blue);
            Console.ReadLine();
            Console.ResetColor();
        }


    }
}
