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
        private TaskProgress _progress = TaskProgress.BackLog;
        private TaskPriority _priority = TaskPriority.Normal;

        public string Title
        {
            get => _title;
            set
            {
                SetProperty(ref _title, value);
                _logger.Log(LogLevel.INFO, $"Title is now: {_title}");
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
                _logger.Log(LogLevel.INFO, $"Description is now: {_description}");
            }
        }

        public DateTime Deadline
        {
            get => _deadline;
            set
            {
                SetProperty(ref _deadline, value);
                _logger.Log(LogLevel.INFO, $"Deadline is now: {_deadline}");
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                SetProperty(ref _startDate, value);
                _logger.Log(LogLevel.INFO, $"Start date is now: {_startDate}");
            }
        }

        public TaskProgress Progress
        {
            get => _progress;
            set
            {
                SetProperty(ref _progress, value);
                _logger.Log(LogLevel.INFO, $"Task progress is now: {_progress}");
            }
        }

        public TaskPriority Priority
        {
            get => _priority;
            set
            {
                SetProperty(ref _priority, value);
                _logger.Log(LogLevel.INFO, $"Task priority is now: {_priority}");
            }
        }

        public NewTaskViewModel(ILogger logger)
        {
            _logger = logger;
        }

        public TaskProgress[] ProgressOptions { get; } = Enum.GetValues<TaskProgress>();

        public TaskPriority[] PriorityOptions { get; } = Enum.GetValues<TaskPriority>();

        public ProjectTask CreateProjectTask()
        {
            // add some null check logic
            ProjectTask task = new ProjectTask(
                0,
                _title,
                _description,
                _startDate,
                _deadline,
                _progress,
                _priority
            );

            return task;
        }
    }
}
