using System;

namespace TokenLogger
{
    internal static class SetConsoleColor
    {
        public static void Color(ConsoleColor color) => Console.ForegroundColor = color;
    }
}