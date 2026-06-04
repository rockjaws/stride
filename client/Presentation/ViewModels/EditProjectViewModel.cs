using System.Collections.ObjectModel;

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class EditProjectViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private readonly Project _originalProject;
    private readonly IUserService _userService;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private DateTime _startDate = DateTime.Today;
    private DateTime _deadline = DateTime.Today;
    private ObservableCollection<AssignableMember> _assignableMembers = [];

    public ObservableCollection<AssignableMember> AssignableMembers
    {
        get => _assignableMembers;
        set => SetProperty(ref _assignableMembers, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public DateTime StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTime Deadline
    {
        get => _deadline;
        set => SetProperty(ref _deadline, value);
    }

    public EditProjectViewModel(ILogger logger, Project project, IUserService userService)
    {
        _logger = logger;
        _originalProject = project;
        _userService = userService;
        _title = project.Title;
        _description = project.Description;
        _startDate = project.StartDate;
        _deadline = project.Deadline;
        _ = GetUsersAsync();
    }

    private async Task GetUsersAsync()
    {
        var users = await _userService.GetUsersAsync();
        _logger.Log(LogLevel.INFO, $"Got {users.Count} users for project assignment");

        AssignableMembers = new ObservableCollection<AssignableMember>(
            users.Select(u => new AssignableMember(u, _originalProject))
        );
    }

    public Project? UpdateProject()
    {
        if (_originalProject.Id == null)
        {
            _logger.Log(LogLevel.ERROR, "Cannot update a project before it has been saved.");
            return null;
        }

        _logger.Log(LogLevel.INFO, $"Prepared Project Update: {_originalProject.Id}");
        return new Project(
            _originalProject.Id,
            _title,
            _description,
            _startDate,
            _deadline,
            _originalProject.ChatChannels,
            _originalProject.IsArchived,
            _originalProject.Tasks,
            AssignableMembers
                .Where(member => member.IsAssigned)
                .Select(member => member.User)
                .ToList()
        );
    }
}
