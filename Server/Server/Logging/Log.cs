using System;
using System.Collections.Generic;
using System.Linq;
namespace Server {
    public static class Log {
        public delegate void OnNewLogMessage(LogType type, string message);
        public static event OnNewLogMessage NewLogMessage;

        public static void Debug(string message) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();

            NewLogMessage?.Invoke(LogType.Debug, message);
        }

        public static void Warning(string message) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();

            NewLogMessage?.Invoke(LogType.Warning, message);
        }

        public static void Error(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            NewLogMessage?.Invoke(LogType.Error, message);
        }
    }
}
