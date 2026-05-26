using System;
using System.Windows.Input;

using client.Presentation.ViewModels;

namespace client.Presentation.Commands;

public class SendMessageCommand : ICommand
{
    private readonly ChatViewModel _viewModel;

    public SendMessageCommand(ChatViewModel viewModel)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }

    public bool CanExecute(object? parameter)
    {
        return _viewModel.SelectedChannel != null &&
               !string.IsNullOrWhiteSpace(_viewModel.MessageInputText);
    }

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
