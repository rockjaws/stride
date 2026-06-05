using System.Collections.ObjectModel;

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Common;

namespace client.Presentation.ViewModels
{
    public class NewTaskViewModel : ObservableObject
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly int _projectId;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private DateTime _startDate = DateTime.Today;
        private DateTime _deadline = DateTime.Today.AddDays(7);
        private TaskProgress _progress = TaskProgress.Backlog;
        private TaskPriority _priority = TaskPriority.Normal;
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

        public DateTime Deadline
        {
            get => _deadline;
            set => SetProperty(ref _deadline, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public TaskProgress Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public TaskPriority Priority
        {
            get => _priority;
            set => SetProperty(ref _priority, value);
        }

        public TaskProgress[] ProgressOptions { get; } = Enum.GetValues<TaskProgress>();

        public TaskPriority[] PriorityOptions { get; } = Enum.GetValues<TaskPriority>();

        public NewTaskViewModel(ILogger logger, IUserService userService, int projectId)
        {
            _logger = logger;
            _userService = userService;
            _projectId = projectId;
            _ = GetUsersAsync();
        }

        private async Task GetUsersAsync()
        {
            var users = await _userService.GetUsersAsync(_projectId);
            _logger.Log(LogLevel.INFO, $"Got {users.Count} users for new task assignment");

            AssignableMembers = new ObservableCollection<AssignableMember>(
                users.Select(u => new AssignableMember(u, false))
            );
        }

        public bool Validate(out string validationMessage)
        {
            if (string.IsNullOrWhiteSpace(_title))
            {
                validationMessage = "Task title is required.";
                _logger.Log(LogLevel.WARNING, "New task validation failed: title is required.");
                return false;
            }

            validationMessage = string.Empty;
            return true;
        }

        public ProjectTask CreateProjectTask(int projectId)
        {
            _logger.Log(LogLevel.INFO, $"Prepared New Task For Project {projectId}: {_title}");

            ProjectTask task = new ProjectTask(
                null,
                _title.Trim(),
                _description,
                _startDate,
                _deadline,
                _progress,
                _priority,
                projectId
            );

            task.UsersAssigned = AssignableMembers
                .Where(member => member.IsAssigned)
                .Select(member => member.User)
                .ToList();

            return task;
        }
    }
}
