using System.Diagnostics;
using client.Application.Interfaces;
using client.Domain.Enum;

namespace client.Infrastructure.Logging;

public class Logger : ILogger
{
    public Logger() { }

    public void Log(LogLevel level, string msg)
    {
        Debug.WriteLine($"[{level}] - {msg}");
    }
}
