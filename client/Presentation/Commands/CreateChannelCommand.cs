// =============================================================================
// Author: Nicolai and Oliver
// =============================================================================

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.ViewModels;
using client.Presentation.Views;

namespace client.Presentation.Commands;

public class CreateChannelCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly Func<ChatChannel, Task> _createChannelAsync;
    private readonly Func<int?> _getProjectId;

    // Author: Nicolai
    public CreateChannelCommand(
        ILogger logger,
        Func<ChatChannel, Task> createChannelAsync,
        Func<int?> getProjectId
    )
    {
        _logger = logger;
        _createChannelAsync = createChannelAsync;
        _getProjectId = getProjectId;
    }

    // Author: Nicolai and Oliver
    public async void Execute(object? param)
    {
        if (!CanExecute(param))
        {
            _logger.Log(LogLevel.WARNING, "CreateChannelCommand cannot execute without a selected project.");
            return;
        }

        var vm = new NewChatViewModel(_logger);
        var window = new NewChannelWindow { DataContext = vm };

        if (window.ShowDialog() == true)
        {
            try
            {
                int projectId = _getProjectId()
                    ?? throw new InvalidOperationException("No project selected.");

                ChatChannel chatChannel = vm.CreateChannel(projectId);
                await _createChannelAsync(chatChannel);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.ERROR, $"Failed to create channel: {ex.Message}");
            }
            return;
        }

        _logger.Log(LogLevel.INFO, "Create channel dialog cancelled.");
    }

    // Author: Nicolai
    public void Undo() { }

    // Author: Nicolai
    public bool CanExecute(object? param) => _getProjectId() != null;

    public event EventHandler? CanExecuteChanged;

    // Author: Nicolai
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
