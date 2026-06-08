// =============================================================================
// Author: Nicolaj
// =============================================================================

using System;
using System.Windows.Input;

using client.Presentation.ViewModels;

namespace client.Presentation.Commands;

public class SendMessageCommand : ICommand
{
    private readonly ChatViewModel _viewModel;

    // Author: Nicolaj
    public SendMessageCommand(ChatViewModel viewModel)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }

    // Author: Nicolaj
    public bool CanExecute(object? parameter)
    {
        return _viewModel.SelectedChannel != null &&
               !string.IsNullOrWhiteSpace(_viewModel.MessageInputText);
    }

    // Author: Nicolaj
    public async void Execute(object? parameter)
    {
        await _viewModel.SendMessageAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
