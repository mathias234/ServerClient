using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    public static class Log {
        private static List<LogMessage> _logMessages = new List<LogMessage>();

        public static void Debug(string message) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();

            _logMessages.Add(new LogMessage(LogType.Debug, message));
        }

        public static void Warning(string message) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();

            _logMessages.Add(new LogMessage(LogType.Warning, message));
        }

        public static void Error(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            _logMessages.Add(new LogMessage(LogType.Error, message));
        }

        public static List<LogMessage> GetLog() {
            return _logMessages;
        }
    }
}
