// =============================================================================
// Author: Nicolai and Oliver
// =============================================================================

using System.Collections.ObjectModel;

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class NewProjectViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private readonly IUserService _userService;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private DateTime _startDate = DateTime.Today;
    private DateTime _deadline = DateTime.Today.AddDays(14);
    private ObservableCollection<AssignableMember> _assignableMembers = [];

    public ObservableCollection<AssignableMember> AssignableMembers
    {
        get => _assignableMembers;
        set => SetProperty(ref _assignableMembers, value);
    }

    public string Title
    {
        get => _title;
        set
        {
            if (SetProperty(ref _title, value))
                OnPropertyChanged(nameof(CanCreate));
        }
    }

    public bool CanCreate => !string.IsNullOrWhiteSpace(_title);

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

    // Author: Oliver
    public NewProjectViewModel(ILogger logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
        _ = GetUsersAsync();
    }

    // Author: Oliver
    private async Task GetUsersAsync()
    {
        var users = await _userService.GetUsersAsync();
        _logger.Log(LogLevel.INFO, $"Got {users.Count} users for new project assignment");

        AssignableMembers = new ObservableCollection<AssignableMember>(
            users.Select(u => new AssignableMember(u, u.Id == _userService.Id))
        );
    }

    // Author: Oliver
    public bool Validate(out string validationMessage)
    {
        if (string.IsNullOrWhiteSpace(_title))
        {
            validationMessage = "Project title is required.";
            _logger.Log(LogLevel.WARNING, "New project validation failed: title is required.");
            return false;
        }

        validationMessage = string.Empty;
        return true;
    }

    // Author: Nicolai and Oliver
    public Project CreateProject()
    {
        _logger.Log(LogLevel.INFO, $"Prepared New Project: {_title}");

        return new Project(
            null,
            _title.Trim(),
            _description,
            _startDate,
            _deadline,
            [],
            false,
            [],
            AssignableMembers
                .Where(member => member.IsAssigned)
                .Select(member => member.User)
                .ToList()
        );
    }
}
