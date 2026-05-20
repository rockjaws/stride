using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class NewProjectViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private DateTime _startDate = DateTime.Today;
    private DateTime _deadline = DateTime.Today.AddDays(14);

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

    public NewProjectViewModel(ILogger logger)
    {
        _logger = logger;
    }

    public Project CreateProject()
    {
        _logger.Log(LogLevel.INFO, $"Prepared New Project: {_title}");

        return new Project(
            null,
            _title,
            _description,
            _startDate,
            _deadline,
            [],
            []
        );
    }
}
