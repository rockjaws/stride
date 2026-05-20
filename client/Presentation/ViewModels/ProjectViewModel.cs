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
    private ProjectTask? _selectedTask;
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

    public ProjectTask? SelectedTask
    {
        get => _selectedTask;
        set { SetProperty(ref _selectedTask, value); }
    }

    public ObservableCollection<Project> ListOfProjects { get; }

    public CreateNewTaskCommand CreateNewTaskCommand { get; }

    public ShowSelectedTaskCommand ShowSelectedTaskCommand { get; }

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
        ShowSelectedTaskCommand = new ShowSelectedTaskCommand(
            _logger,
            _taskService,
            UpdateTask,
            RemoveDeletedTask
        );
        _ = GetProjectsAsync();
    }

    private void LoadTasks(Project currentProject)
    {
        foreach (ProjectTask task in currentProject.Tasks)
        {
            GetTaskCollection(task.Progress).Add(task);
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

        GetTaskCollection(task.Progress).Add(task);
        SelectedProject?.Tasks.Add(task);
    }

    private void UpdateTask(ProjectTask task)
    {
        if (task == null)
            return;

        if (
            SelectedProject?.Tasks.FirstOrDefault(p => p.Id == task.Id)
            is not ProjectTask existingTask
        )
            return;

        GetTaskCollection(existingTask.Progress).Remove(existingTask);
        GetTaskCollection(task.Progress).Add(task);
        ReplaceSelectedProjectTask(existingTask, task);
    }

    public async Task DeleteTaskAsync(ProjectTask task)
    {
        if (task.Id is not int id)
            return;

        try
        {
            await _taskService.DeleteTaskAsync(id);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Delete Task {id}: {ex.Message}");
            return;
        }

        RemoveDeletedTask(task);
        _logger.Log(LogLevel.INFO, $"Deleted Task {id}");
    }

    private void RemoveDeletedTask(ProjectTask task)
    {
        GetTaskCollection(task.Progress).Remove(task);

        if (SelectedProject != null)
        {
            ITask? projectTask = SelectedProject.Tasks.FirstOrDefault(t => t.Id == task.Id);
            if (projectTask != null)
                SelectedProject.Tasks.Remove(projectTask);
        }

        if (SelectedTask?.Id == task.Id)
            SelectedTask = null;
    }

    public async Task UpdateTaskAsync(ProjectTask task)
    {
        if (task == null)
            return;

        try
        {
            await _taskService.UpdateTaskAsync(task);
            UpdateTask(task);
            _logger.Log(LogLevel.INFO, $"Updated Task {task.Id}");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Update Task {task.Id}: {ex.Message}");
        }
    }

    public async Task MoveTaskAsync(ProjectTask task, TaskProgress progress)
    {
        if (task.Progress == progress)
            return;

        try
        {
            ProjectTask movedTask = await _taskService.MoveTaskAsync(task, progress);

            GetTaskCollection(task.Progress).Remove(task);
            GetTaskCollection(movedTask.Progress).Add(movedTask);
            ReplaceSelectedProjectTask(task, movedTask);

            _logger.Log(LogLevel.INFO, $"Moved Task {task.Id} To {progress}");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Move Task {task.Id}: {ex.Message}");
        }
    }

    private ObservableCollection<ProjectTask> GetTaskCollection(TaskProgress progress)
    {
        return progress switch
        {
            TaskProgress.Backlog => BacklogTasks,
            TaskProgress.InProgress => InProgressTasks,
            TaskProgress.Review => InReviewTasks,
            TaskProgress.Done => FinishedTasks,
            _ => throw new UnknownTaskProgressException(progress),
        };
    }

    private void ReplaceSelectedProjectTask(ProjectTask originalTask, ProjectTask movedTask)
    {
        if (SelectedProject == null)
            return;

        int taskIndex = SelectedProject.Tasks.FindIndex(task => task.Id == originalTask.Id);
        if (taskIndex >= 0)
        {
            SelectedProject.Tasks[taskIndex] = movedTask;
            return;
        }

        SelectedProject.Tasks.Add(movedTask);
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
