using System;

namespace RaptorOS.Utils;

public static class Logger
{
    public static void LogInfo(string message) => Log($"Info: {message}", ConsoleColor.Green);

    public static void LogError(string message) => Log($"Error: {message}", ConsoleColor.Red);

    public static void LogWarning(string message) =>
        Log($"Warning: {message}", ConsoleColor.Yellow);

    public static void Log(string message, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
