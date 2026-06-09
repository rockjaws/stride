// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Commands;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class ChatViewModel : ObservableObject, IDisposable
{
    private readonly ILogger _logger;
    private readonly IProjectService _projectService;
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;
    private readonly DispatcherTimer _refreshTimer;

    public ObservableCollection<IProject> Projects { get; } = new();
    public ObservableCollection<IChatChannel> ChatChannels { get; } = new();
    public ObservableCollection<IMessage> Messages { get; } = new();

    private IProject _selectedProject;
    public IProject SelectedProject
    {
        get => _selectedProject;
        set
        {
            if (SetProperty(ref _selectedProject, value))
            {
                _ = LoadChannelsForSelectedProjectAsync();
            }
        }
    }

    private IChatChannel _selectedChannel;
    public IChatChannel SelectedChannel
    {
        get => _selectedChannel;
        set
        {
            if (SetProperty(ref _selectedChannel, value))
            {
                _ = LoadMessagesForSelectedChannelAsync();
            }
        }
    }

    private string _messageInputText = string.Empty;
    public string MessageInputText
    {
        get => _messageInputText;
        set => SetProperty(ref _messageInputText, value);
    }

    public ICommand SendMessageCommand { get; }
    public ICommand CreateChannelCommand { get; }
    public DeleteChannelCommand DeleteChannelCommand { get; }

    // Author: Nicolaj and Oliver
    public ChatViewModel(
        ILogger logger,
        IProjectService projectService,
        IMessageService messageService,
        IUserService userService
    )
    {
        _logger = logger;
        _projectService = projectService;
        _messageService = messageService;
        _userService = userService;

        SendMessageCommand = new SendMessageCommand(this);
        CreateChannelCommand = new CreateChannelCommand(
            _logger,
            CreateChannelAsync,
            () => SelectedProject?.Id
        );
        DeleteChannelCommand = new DeleteChannelCommand(
            _logger,
            () => true,
            () => SelectedChannel as ChatChannel,
            DeleteChannelAsync
        );

        _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        _refreshTimer.Tick += async (s, e) => await PollLatestMessageAsync();

        _ = LoadDataAsync();
    }

    // Author: Nicolaj and Oliver
    // Loads projects, chooses the initial selection, and starts polling when a channel is available.
    private async Task LoadDataAsync()
    {
        try
        {
            var projects = await _projectService.GetProjectsAsync();
            Projects.Clear();
            foreach (var project in projects)
            {
                Projects.Add(project);
            }

            if (Projects.Count > 0)
            {
                // Set the backing field directly to avoid triggering a duplicate asynchronous load.
                _selectedProject = Projects[0];
                OnPropertyChanged(nameof(SelectedProject));

                await LoadChannelsForSelectedProjectAsync();
            }
            else
            {
                _logger.Log(
                    LogLevel.WARNING,
                    "[LoadDataAsync] No projects returned — channel and message lists will remain empty"
                );
            }
        }
        catch (Exception ex)
        {
            _logger.Log(
                LogLevel.ERROR,
                $"[LoadDataAsync] Exception: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}"
            );
        }
        finally
        {
            if (SelectedChannel != null)
            {
                _refreshTimer.Start();
            }
            else
            {
                _logger.Log(
                    LogLevel.WARNING,
                    "[LoadDataAsync] SelectedChannel is null after load — refresh timer will not start"
                );
            }
        }
    }

    // Author: Nicolaj and Oliver
    // Rebuilds the channel list from the selected project's already-loaded channel data.
    private async Task LoadChannelsForSelectedProjectAsync()
    {
        ChatChannels.Clear();
        Messages.Clear();

        if (SelectedProject == null)
        {
            return;
        }

        try
        {
            var channels = SelectedProject.ChatChannels;
            if (channels == null || channels.Count == 0)
            {
                return;
            }

            foreach (var channel in channels)
            {
                ChatChannels.Add(channel);
            }
            SelectedChannel = ChatChannels[0];
        }
        catch (Exception ex)
        {
            _logger.Log(
                LogLevel.WARNING,
                $"[LoadChannelsForSelectedProjectAsync] Exception: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}"
            );
        }
    }

    // Author: Nicolaj and Oliver
    // Replaces the visible conversation whenever the selected channel changes.
    public async Task LoadMessagesForSelectedChannelAsync()
    {
        if (SelectedChannel == null)
        {
            Messages.Clear();
            return;
        }

        try
        {
            var chatHistory = await _messageService.GetMessagesAsync(SelectedChannel.Id);
            Messages.Clear();
            foreach (var message in chatHistory)
            {
                Messages.Add(message);
            }
        }
        catch (Exception ex)
        {
            _logger.Log(
                LogLevel.ERROR,
                $"[LoadMessagesForSelectedChannelAsync] Exception: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}"
            );
        }
    }

    // Author: Nicolaj and Oliver
    // Persists the current input and appends the API's canonical message response.
    public async Task SendMessageAsync()
    {
        _logger.Log(
            LogLevel.WARNING,
            $"[SendMessageAsync] Attempting to send on channel: {SelectedChannel?.Id ?? null}"
        );

        if (SelectedChannel == null || string.IsNullOrWhiteSpace(MessageInputText))
        {
            return;
        }
        try
        {
            var sentMessage = await _messageService.SendMessageAsync(
                SelectedChannel.Id,
                MessageInputText,
                _userService.Id
            );
            Messages.Add(sentMessage);
            MessageInputText = string.Empty;
        }
        catch (Exception ex)
        {
            _logger.Log(
                LogLevel.ERROR,
                $"[SendMessageAsync] Exception: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}"
            );
        }
    }

    // Author: Nicolaj and Oliver
    // Polls the selected channel without allowing stale responses to overwrite a newer selection.
    private async Task PollLatestMessageAsync()
    {
        if (SelectedChannel == null)
        {
            return;
        }

        try
        {
            // Prevent overlapping timer ticks while the current request is in flight.
            _refreshTimer.Stop();

            // Discard the response if the user changes channels before the request completes.
            var currentChannelId = SelectedChannel.Id;
            var serverMessages = await _messageService.GetMessagesAsync(currentChannelId);

            if (SelectedChannel?.Id != currentChannelId)
            {
                return;
            }

            if (serverMessages.Count != Messages.Count)
            {
                // Polling uses count as a cheap change detector; a mismatch requires a full refresh.
                Messages.Clear();
                foreach (var message in serverMessages)
                {
                    Messages.Add(message);
                }
            }
            else
            {
                _logger.Log(LogLevel.WARNING, "[PollLatestMessageAsync] No changes detected");
            }
        }
        catch (Exception ex)
        {
            _logger.Log(
                LogLevel.ERROR,
                $"[PollLatestMessageAsync] Exception: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}"
            );
        }
        finally
        {
            if (SelectedChannel != null)
            {
                _refreshTimer.Start();
            }
            else
            {
                _logger.Log(
                    LogLevel.WARNING,
                    "[PollLatestMessageAsync] SelectedChannel is null in finally — timer will not restart"
                );
            }
        }
    }

    // Author: Nicolaj
    public void AddProject(IProject project)
    {
        Projects.Add(project);
    }

    // Author: Oliver
    private async Task DeleteChannelAsync(IChatChannel channel)
    {
        try
        {
            await _projectService.DeleteChannelAsync(channel.ProjectId, channel.Id);

            ChatChannels.Remove(channel);
            SelectedChannel = ChatChannels?.ElementAtOrDefault(0);

            _logger.Log(LogLevel.INFO, $"Successfully deleted channel {channel.Id}");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed to delete channel: {ex.Message}");
        }
    }

    // Author: Nicolaj
    private async Task CreateChannelAsync(IChatChannel channel)
    {
        try
        {
            var saved = await _projectService.CreateChannelAsync(channel.ProjectId, channel.Name);
            ChatChannels.Add(saved);
            SelectedChannel = saved;
            _logger.Log(LogLevel.INFO, $"Created channel '{saved.Name}' (id {saved.Id})");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed to create channel: {ex.Message}");
        }
    }

    // Author: Nicolaj
    public void Dispose()
    {
        _refreshTimer.Stop();
        GC.SuppressFinalize(this);
    }
}
