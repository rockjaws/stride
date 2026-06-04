using System.Diagnostics;
using System.IO;

using client.Application.Interfaces;
using client.Domain.Enum;

namespace client.Infrastructure.Logging;

public class Logger : ILogger
{
    private static readonly object FileLock = new();

    public string LogFilePath { get; }

    public Logger(string? logFilePath = null)
    {
        LogFilePath = logFilePath ?? GetDefaultLogFilePath();
        Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath)!);
        File.WriteAllText(LogFilePath, string.Empty);

        Log(LogLevel.INFO, $"Logger initialized. Log file: {LogFilePath}");
    }

    public void Log(LogLevel level, string msg)
    {
        var entry = $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz} [{level}] {msg}";
        Debug.WriteLine(entry);

        try
        {
            lock (FileLock)
            {
                using var stream = new FileStream(
                    LogFilePath,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.ReadWrite
                );
                using var writer = new StreamWriter(stream);
                writer.WriteLine(entry);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{LogLevel.ERROR}] Failed to write log file: {ex.Message}");
        }
    }

    private static string GetDefaultLogFilePath()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, "Stride", "logs", "stride.log");
    }
}
