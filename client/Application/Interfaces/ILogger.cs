// =============================================================================
// Author: Oliver
// =============================================================================

using client.Domain.Enum;

namespace client.Application.Interfaces;

public interface ILogger
{
    // Author: Oliver
    void Log(LogLevel level, string msg);
}
