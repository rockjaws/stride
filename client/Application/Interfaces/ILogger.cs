using client.Domain.Enum;

namespace client.Application.Interfaces;

public interface ILogger
{
    void Log(LogLevel level, string msg) { }
}
