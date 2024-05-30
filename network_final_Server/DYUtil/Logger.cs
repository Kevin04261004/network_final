using System.Diagnostics;

namespace DYUtil
{
    public static class Logger
    {
        public static void LogError(string header, string str, bool ProgramExit = false)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{header}] {str}");
            if (ProgramExit)
            {
                Environment.Exit(0);
            }
        }
        public static void Log(string header, string str, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"[{header}] {str}");
        }
    }
}
