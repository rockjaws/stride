using Microsoft.Extensions.Logging;

namespace api.Infrastructure.Logging;

public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly object _fileLock = new();
    private readonly string _logFilePath;

    public FileLoggerProvider(string? logFilePath = null)
    {
        _logFilePath = logFilePath ?? GetDefaultLogFilePath();
        Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);
        File.WriteAllText(_logFilePath, string.Empty);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, _logFilePath, _fileLock);
    }

    public void Dispose() { }

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

        public FileLogger(string categoryName, string logFilePath, object fileLock)
        {
            _categoryName = categoryName;
            _logFilePath = logFilePath;
            _fileLock = fileLock;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            // EF command logging is intentionally excluded because it dominates the application log.
            if (ExcludedCategories.Any(category => _categoryName.StartsWith(category)))
                return false;

            return logLevel != Microsoft.Extensions.Logging.LogLevel.None;
        }

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

            // All category loggers share this lock because they append to the same file.
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
        public static NullScope Instance { get; } = new();

        public void Dispose() { }
    }
}
