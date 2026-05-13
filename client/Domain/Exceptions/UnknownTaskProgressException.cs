using client.Domain.Enum;

namespace client.Domain.Exceptions;

public class UnknownTaskProgressException : Exception
{
    public TaskProgress Progress { get; }

    public UnknownTaskProgressException(TaskProgress progress)
        : base($"Unknown Task Progress Value: {progress}")
    {
        Progress = progress;
    }
}
