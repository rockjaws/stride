using System.Collections.ObjectModel;
using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Exceptions;
using client.Domain.Models;
using client.Presentation.Commands;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class ProjectViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private readonly IProjectService _projectService;
    private readonly ITaskService _taskService;
    private Project? _selectedProject;
    private ObservableCollection<ProjectTask> _backlogTasks = [];
    private ObservableCollection<ProjectTask> _inProgressTasks = [];
    private ObservableCollection<ProjectTask> _inReviewTasks = [];
    private ObservableCollection<ProjectTask> _finishedTasks = [];

    public ObservableCollection<ProjectTask> BacklogTasks
    {
        get => _backlogTasks;
        set => SetProperty(ref _backlogTasks, value);
    }

    public ObservableCollection<ProjectTask> InProgressTasks
    {
        get => _inProgressTasks;
        set => SetProperty(ref _inProgressTasks, value);
    }

    public ObservableCollection<ProjectTask> InReviewTasks
    {
        get => _inReviewTasks;
        set => SetProperty(ref _inReviewTasks, value);
    }

    public ObservableCollection<ProjectTask> FinishedTasks
    {
        get => _finishedTasks;
        set => SetProperty(ref _finishedTasks, value);
    }

    public Project? SelectedProject
    {
        get => _selectedProject;
        set
        {
            SetProperty(ref _selectedProject, value);
            CreateNewTaskCommand.RaiseCanExecuteChanged();
            ClearTaskColumns();

            if (_selectedProject == null)
                return;

            _logger.Log(LogLevel.INFO, $"New Project Selected: {_selectedProject.Title}");
            LoadTasks(_selectedProject);
        }
    }

    public ObservableCollection<Project> ListOfProjects { get; }

    public CreateNewTaskCommand CreateNewTaskCommand { get; }

    public ProjectViewModel(
        ILogger logger,
        IProjectService projectService,
        ITaskService taskService
    )
    {
        _logger = logger;
        _projectService = projectService;
        _taskService = taskService;
        ListOfProjects = [];
        BacklogTasks = [];
        InProgressTasks = [];
        InReviewTasks = [];
        FinishedTasks = [];
        CreateNewTaskCommand = new CreateNewTaskCommand(
            _logger,
            _taskService,
            AddCreatedTask,
            () => _selectedProject != null,
            () => _selectedProject?.Id
        );
        _ = GetProjectsAsync();
    }

    private void LoadTasks(Project currentProject)
    {
        foreach (ProjectTask task in currentProject.Tasks)
        {
            var collection = task.Progress switch
            {
                TaskProgress.BackLog => BacklogTasks,
                TaskProgress.InProgress => InProgressTasks,
                TaskProgress.Review => InReviewTasks,
                TaskProgress.Done => FinishedTasks,
                _ => throw new UnknownTaskProgressException(task.Progress),
            };

            collection.Add(task);
            _logger.Log(LogLevel.INFO, $"Loaded {task.Id}");
        }
    }

    private void ClearTaskColumns()
    {
        BacklogTasks.Clear();
        InProgressTasks.Clear();
        InReviewTasks.Clear();
        FinishedTasks.Clear();
    }

    private void AddCreatedTask(ProjectTask task)
    {
        if (task == null)
            return;

        var collection = task.Progress switch
        {
            TaskProgress.BackLog => BacklogTasks,
            TaskProgress.InProgress => InProgressTasks,
            TaskProgress.Review => InReviewTasks,
            TaskProgress.Done => FinishedTasks,
            _ => throw new UnknownTaskProgressException(task.Progress),
        };

        collection.Add(task);
        SelectedProject?.Tasks.Add(task);
    }

    public void AddCreatedProject(Project project)
    {
        if (project == null)
            return;

        ListOfProjects.Add(project);
        SelectedProject = project;
    }

    public async Task GetProjectsAsync()
    {
        try
        {
            var projects = await _projectService.GetProjectsAsync();

            ListOfProjects.Clear();
            foreach (var project in projects)
            {
                ListOfProjects.Add(project);
            }
            _logger.Log(LogLevel.INFO, "All Projects Fetched Succesfully.");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Load Projects: {ex.Message}");
        }
    }
}
