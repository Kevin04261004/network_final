using DYUtil;

namespace GameLogicServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Thread serverThread = new Thread(LogicServer.ServerFunction)
            {
                IsBackground = true
            };
            serverThread.Start();

            IPFinder.TryGetMyIPv4(out string myIp);
            Logger.Log($"{myIp}:{LogicServer.PORT_NUM}", "Game Server Started...", ConsoleColor.Green);
            Logger.Log("Info", "Enter To Exit Server", ConsoleColor.Blue);
            Console.ReadLine();
            Console.ResetColor();
        }


    }
}
