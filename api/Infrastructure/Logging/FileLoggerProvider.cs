using Microsoft.Extensions.Logging;

namespace api.Infrastructure.Logging;

public sealed class FileLoggerProvider : ILoggerProvider
{
    // Author: Nicolai and Oliver
    private readonly object _fileLock = new();
    private readonly string _logFilePath;

    // Author: Nicolai and Oliver
    public FileLoggerProvider(string? logFilePath = null)
    {
        _logFilePath = logFilePath ?? GetDefaultLogFilePath();
        Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);
        File.WriteAllText(_logFilePath, string.Empty);
    }

    // Author: Nicolai and Oliver
    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, _logFilePath, _fileLock);
    }

    // Author: Nicolai and Oliver
    public void Dispose() { }

    // Author: Nicolai and Oliver
    public static string GetDefaultLogFilePath()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, "Stride", "logs", "stride.log");
    }

    private sealed class FileLogger : ILogger
    {
        private static readonly string[] ExcludedCategories =
        [
            "Microsoft.EntityFrameworkCore.Database.Command",
        ];

        private readonly string _categoryName;
        private readonly object _fileLock;
        private readonly string _logFilePath;

        // Author: Nicolai and Oliver
        public FileLogger(string categoryName, string logFilePath, object fileLock)
        {
            _categoryName = categoryName;
            _logFilePath = logFilePath;
            _fileLock = fileLock;
        }

        // Author: Nicolai and Oliver
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return NullScope.Instance;
        }

        // Author: Nicolai and Oliver
        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            if (ExcludedCategories.Any(category => _categoryName.StartsWith(category)))
                return false;

            return logLevel != Microsoft.Extensions.Logging.LogLevel.None;
        }

        // Author: Nicolai and Oliver
        public void Log<TState>(
            Microsoft.Extensions.Logging.LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter
        )
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            if (string.IsNullOrWhiteSpace(message) && exception == null)
                return;

            var entry =
                $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz} [{logLevel}] {_categoryName} [{eventId.Id}] {message}";

            if (exception != null)
                entry += Environment.NewLine + exception;

            lock (_fileLock)
            {
                using var stream = new FileStream(
                    _logFilePath,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.ReadWrite
                );
                using var writer = new StreamWriter(stream);
                writer.WriteLine(entry);
            }
        }
    }

    private sealed class NullScope : IDisposable
    {
        // Author: Nicolai and Oliver
        public static NullScope Instance { get; } = new();

        // Author: Nicolai and Oliver
        public void Dispose() { }
    }
}
