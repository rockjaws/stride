using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;

namespace client.Presentation.Commands;

public class DeleteChannelCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly Func<bool>? _canExecute;
    private readonly Func<ChatChannel?>? _getChannel;

    private readonly Func<ChatChannel, Task> _deleteChannel;

    public DeleteChannelCommand(
        ILogger logger,
        Func<bool>? canExecute,
        Func<ChatChannel?>? getChannel,
        Func<ChatChannel, Task> deleteChannel
    )
    {
        _logger = logger;
        _canExecute = canExecute;
        _getChannel = getChannel;
        _deleteChannel = deleteChannel;
    }

    public async void Execute(object? parameter)
    {
        var channel = parameter as ChatChannel ?? _getChannel?.Invoke();

        if (channel != null)
        {
            try
            {
                await _deleteChannel(channel);
            }
            catch (Exception ex)
            {
                _logger.Log(
                    LogLevel.ERROR,
                    $"Failed to execute DeleteChannelCommand: {ex.Message}"
                );
            }
        }
    }

    public bool CanExecute(object? parameter)
    {
        bool canExecute = _canExecute?.Invoke() ?? true;

        return parameter is ChatChannel || canExecute;
    }

    public void Undo() { }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
