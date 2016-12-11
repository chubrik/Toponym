using System;

namespace Toponym.Core {
    public class Utils {

        private const ConsoleColor DefaultColor = ConsoleColor.DarkGray;

        public static bool IsDebug() {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        public static void Log(string message) => Console.WriteLine($"{DateTimeOffset.Now:HH:mm:ss.fff} - {message}");

        public static void LogImportant(string message) => Log(message, ConsoleColor.White);

        public static void LogSuccess(string message) => Log(message, ConsoleColor.Green);

        public static void LogWarning(string message) => Log(message, ConsoleColor.Yellow);

        public static void LogError(string message) => Log(message, ConsoleColor.Red);

        public static void LogRead(string message) => Log(message, ConsoleColor.Cyan);

        public static void LogSaved(string message) => Log(message, ConsoleColor.Magenta);

        private static void Log(string message, ConsoleColor color) {
            Console.ForegroundColor = color;
            Log(message);
            Console.ForegroundColor = DefaultColor;
        }

        public static string GetFileName(string filePath) {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            return filePath.Substring(filePath.LastIndexOf("/", StringComparison.Ordinal) + 1);
        }
    }
}
