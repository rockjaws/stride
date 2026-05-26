using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Presentation.Commands;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class ChatViewModel : ObservableObject
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
                _logger.Log(LogLevel.WARNING, $"[SelectedProject] Changed to: {value?.Id} — '{value?.Description ?? "null"}'");
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

    public ChatViewModel(ILogger logger, IProjectService projectService, IMessageService messageService, IUserService userService)
    {
        _logger = logger;
        _projectService = projectService;
        _messageService = messageService;
        _userService = userService;

        _logger.Log(LogLevel.WARNING, "[ChatViewModel] Initialising");

        SendMessageCommand = new SendMessageCommand(this);

        _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        _refreshTimer.Tick += async (s, e) => await PollLatestMessageAsync();

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        _logger.Log(LogLevel.WARNING, "[LoadDataAsync] Starting");

        try
        {
            _logger.Log(LogLevel.WARNING, "[LoadDataAsync] Fetching projects from service");
            var projects = await _projectService.GetProjectsAsync();

            _logger.Log(LogLevel.WARNING, $"[LoadDataAsync] Received {projects?.Count ?? 0} project(s)");

            Projects.Clear();
            foreach (var project in projects)
            {
                _logger.Log(LogLevel.WARNING, $"[LoadDataAsync] Adding project: {project.Id} — '{project.Description}'");
                Projects.Add(project);
            }

            if (Projects.Count > 0)
            {
                _logger.Log(LogLevel.WARNING, $"[LoadDataAsync] Auto-selecting first project: {Projects[0].Id} — '{Projects[0].Description}'");

                _selectedProject = Projects[0];
                OnPropertyChanged(nameof(SelectedProject));

                await LoadChannelsForSelectedProjectAsync();
            }
            else
            {
                _logger.Log(LogLevel.WARNING, "[LoadDataAsync] No projects returned — channel and message lists will remain empty");
            }
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"[LoadDataAsync] Exception: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
        finally
        {
            _logger.Log(LogLevel.WARNING, $"[LoadDataAsync] Complete — Projects: {Projects.Count}, Channels: {ChatChannels.Count}, SelectedChannel: {SelectedChannel?.Id ?? 0}");

            if (SelectedChannel != null)
            {
                _logger.Log(LogLevel.WARNING, "[LoadDataAsync] Starting refresh timer");
                _refreshTimer.Start();
            }
            else
            {
                _logger.Log(LogLevel.WARNING, "[LoadDataAsync] SelectedChannel is null after load — refresh timer will not start");
            }
        }
    }

    private async Task LoadChannelsForSelectedProjectAsync()
    {
        _logger.Log(LogLevel.WARNING, $"[LoadChannelsForSelectedProjectAsync] Starting for project: {SelectedProject?.Id ?? 0}");

        ChatChannels.Clear();
        Messages.Clear();

        if (SelectedProject == null)
        {
            _logger.Log(LogLevel.WARNING, "[LoadChannelsForSelectedProjectAsync] SelectedProject is null — aborting");
            return;
        }

        try
        {
            var channels = SelectedProject.ChatChannels;
            _logger.Log(LogLevel.WARNING, $"[LoadChannelsForSelectedProjectAsync] Channels on project: {channels?.Count ?? 0}");


            if (channels == null || channels.Count == 0)
            {
                _logger.Log(LogLevel.WARNING, $"[LoadChannelsForSelectedProjectAsync] No channels found for project: {SelectedProject.Id} — '{SelectedProject.Description}'");
                return;
            }

            foreach (var channel in channels)
            {
                _logger.Log(LogLevel.WARNING, $"[LoadChannelsForSelectedProjectAsync] Adding channel: {channel.Id} — '{channel.Name}'");
                ChatChannels.Add(channel);
            }

            _logger.Log(LogLevel.WARNING, $"[LoadChannelsForSelectedProjectAsync] Total channels loaded: {ChatChannels.Count} — auto-selecting first");
            SelectedChannel = ChatChannels[0];
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.WARNING, $"[LoadChannelsForSelectedProjectAsync] Exception: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public async Task LoadMessagesForSelectedChannelAsync()
    {
        _logger.Log(LogLevel.WARNING, $"[LoadMessagesForSelectedChannelAsync] Starting for channel: {SelectedChannel?.Id ?? 0}");

        if (SelectedChannel == null)
        {
            _logger.Log(LogLevel.WARNING, "[LoadMessagesForSelectedChannelAsync] SelectedChannel is null — clearing messages");
            Messages.Clear();
            return;
        }

        try
        {
            var chatHistory = await _messageService.GetMessagesAsync(SelectedChannel.Id);

            _logger.Log(LogLevel.WARNING, $"[LoadMessagesForSelectedChannelAsync] Received {chatHistory?.Count ?? 0} message(s) for channel: {SelectedChannel.Id}");

            Messages.Clear();
            foreach (var message in chatHistory)
            {
                Messages.Add(message);
            }
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"[LoadMessagesForSelectedChannelAsync] Exception: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public async Task SendMessageAsync()
    {
        _logger.Log(LogLevel.WARNING, $"[SendMessageAsync] Attempting to send on channel: {SelectedChannel?.Id ?? null}");

        if (SelectedChannel == null || string.IsNullOrWhiteSpace(MessageInputText))
        {
            _logger.Log(LogLevel.WARNING, $"[SendMessageAsync] Aborted — SelectedChannel: {SelectedChannel?.Id ?? 0}, MessageInputText empty: {string.IsNullOrWhiteSpace(MessageInputText)}");
            return;
        }

        try
        {
            var sentMessage = await _messageService.SendMessageAsync(SelectedChannel.Id, MessageInputText, _userService.Id);
            _logger.Log(LogLevel.WARNING, $"[SendMessageAsync] Message sent successfully: {sentMessage?.Id}");

            Messages.Add(sentMessage);
            MessageInputText = string.Empty;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"[SendMessageAsync] Exception: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private async Task PollLatestMessageAsync()
    {
        if (SelectedChannel == null)
        {
            _logger.Log(LogLevel.WARNING, "[PollLatestMessageAsync] SelectedChannel is null — skipping poll");
            return;
        }

        try
        {
            _refreshTimer.Stop();

            var currentChannelId = SelectedChannel.Id;
            _logger.Log(LogLevel.WARNING, $"[PollLatestMessageAsync] Polling channel: {currentChannelId}");

            var serverMessages = await _messageService.GetMessagesAsync(currentChannelId);
            _logger.Log(LogLevel.WARNING, $"[PollLatestMessageAsync] Server returned {serverMessages?.Count ?? 0} message(s), local count: {Messages.Count}");

            if (SelectedChannel?.Id != currentChannelId)
            {
                _logger.Log(LogLevel.WARNING, $"[PollLatestMessageAsync] Channel changed during poll ({currentChannelId} -> {SelectedChannel?.Id ?? 0}) — discarding result");
                return;
            }

            if (serverMessages.Count != Messages.Count)
            {
                _logger.Log(LogLevel.WARNING, $"[PollLatestMessageAsync] Count mismatch — refreshing message list");
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
            _logger.Log(LogLevel.ERROR, $"[PollLatestMessageAsync] Exception: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
        finally
        {
            if (SelectedChannel != null)
            {
                _refreshTimer.Start();
            }
            else
            {
                _logger.Log(LogLevel.WARNING, "[PollLatestMessageAsync] SelectedChannel is null in finally — timer will not restart");
            }
        }
    }
}
