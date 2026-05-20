using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Common;

namespace client.Presentation.ViewModels
{
    public class NewTaskViewModel : ObservableObject
    {
        private readonly ILogger _logger;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private DateTime _startDate = DateTime.Today;
        private DateTime _deadline = DateTime.Today.AddDays(7);
        private TaskProgress _progress = TaskProgress.Backlog;
        private TaskPriority _priority = TaskPriority.Normal;

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

        public NewTaskViewModel(ILogger logger)
        {
            _logger = logger;
        }

        public ProjectTask CreateProjectTask(int projectId)
        {
            _logger.Log(LogLevel.INFO, $"Prepared New Task For Project {projectId}: {_title}");

            ProjectTask task = new ProjectTask(
                null,
                _title,
                _description,
                _startDate,
                _deadline,
                _progress,
                _priority,
                projectId
            );

            return task;
        }
    }
}
