using System.Collections.ObjectModel;

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Exceptions;
using client.Domain.Models;
using client.Presentation.Commands;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class ProjectViewModel : ObservableObject, IDisposable
{
    private readonly ILogger _logger;
    private readonly IProjectService _projectService;
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;
    private Project? _selectedProject;
    private ProjectTask? _selectedTask;
    private ObservableCollection<ProjectTask> _backlogTasks = [];
    private ObservableCollection<ProjectTask> _inProgressTasks = [];
    private ObservableCollection<ProjectTask> _inReviewTasks = [];
    private ObservableCollection<ProjectTask> _finishedTasks = [];
    private bool _isUpdatingGlobalState;

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
            ArchiveProjectCommand.RaiseCanExecuteChanged();
            // The board columns are derived from the selected project's task list.
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

    public ArchiveProjectCommand ArchiveProjectCommand { get; }

    public ProjectViewModel(
        ILogger logger,
        IProjectService projectService,
        ITaskService taskService,
        IUserService userService
    )
    {
        _logger = logger;
        _projectService = projectService;
        _taskService = taskService;
        _userService = userService;
        ListOfProjects = [];
        BacklogTasks = [];
        InProgressTasks = [];
        InReviewTasks = [];
        FinishedTasks = [];
        // Commands receive callbacks so service calls stay in the view model, not in the command.
        CreateNewTaskCommand = new CreateNewTaskCommand(
            _logger,
            CreateTaskAsync,
            () => _selectedProject != null,
            () => _selectedProject?.Id
        );
        ShowSelectedTaskCommand = new ShowSelectedTaskCommand(
            _logger,
            _userService,
            UpdateTaskAsync,
            DeleteTaskAsync
        );
        ArchiveProjectCommand = new ArchiveProjectCommand(
            _logger,
            () => SelectedProject,
            ArchiveProjectAsync
        );

        _taskService.TasksChanged += OnGlobalStateChange;
        _projectService.ProjectsChanged += OnGlobalStateChange;

        _ = GetProjectsAsync();
    }

    private void LoadTasks(Project currentProject)
    {
        // Projects are loaded with their tasks, so selecting a project only needs local grouping.
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

        // Add to both the visible column and the backing project list used when the board reloads.
        GetTaskCollection(task.Progress).Add(task);
        SelectedProject?.Tasks.Add(task);
        _logger.Log(LogLevel.INFO, $"Added Created Task To Board: {task.Id}");
    }

    public async Task CreateTaskAsync(ProjectTask task)
    {
        if (task == null)
            return;

        try
        {
            ProjectTask savedTask = await _taskService.CreateTaskAsync(task);
            AddCreatedTask(savedTask);
            _logger.Log(LogLevel.INFO, $"Created Task {savedTask.Id}: {savedTask.Title}");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Create Task: {ex.Message}");
        }
    }

    private void UpdateTask(ProjectTask task)
    {
        if (task == null)
            return;

        // Replace the card object instead of mutating it so WPF collection bindings refresh reliably.
        if (
            SelectedProject?.Tasks.FirstOrDefault(p => p.Id == task.Id)
            is not ProjectTask existingTask
        )
            return;

        GetTaskCollection(existingTask.Progress).Remove(existingTask);
        GetTaskCollection(task.Progress).Add(task);
        ReplaceSelectedProjectTask(existingTask, task);
        _logger.Log(LogLevel.INFO, $"Updated Task On Board: {task.Id}");
    }

    public async Task DeleteProjectAsync(Project project)
    {
        if (SelectedProject == null)
            return;

        if (project.Id is not int id)
            return;

        try
        {
            await _projectService.DeleteProjectAsync(id);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Delete Project {project.Id}: {ex.Message}");
            return;
        }

        // Remove project after API confirms the delete.
        ListOfProjects.Remove(project);
        _logger.Log(LogLevel.INFO, $"Deleted Project {project.Id}");
    }

    public async Task ArchiveProjectAsync(Project project)
    {
        if (project.Id is not int id)
            return;

        project.Archive(); // Optimistic update, rolled back on failure
        try
        {
            await _projectService.SetProjectArchivedAsync(id, true);
            _logger.Log(LogLevel.INFO, $"Archived Project {id}");
            SelectedProject = null;
            ListOfProjects.Remove(project);
        }
        catch (Exception ex)
        {
            project.UnArchive(); // Rollback
            _logger.Log(LogLevel.ERROR, $"Failed To Archive Project {id}: {ex.Message}");
        }
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

        // Only remove locally after the API confirms the delete.
        RemoveDeletedTask(task);
        _logger.Log(LogLevel.INFO, $"Deleted Task {id}");
    }

    private void RemoveDeletedTask(ProjectTask task)
    {
        var existingTask = SelectedProject?.Tasks.FirstOrDefault(t => t.Id == task.Id);
        // Prefer the project copy because it has the current progress column after moves/edits.
        if (existingTask is ProjectTask projectTask)
            GetTaskCollection(projectTask.Progress).Remove(projectTask);
        else
            GetTaskCollection(task.Progress).Remove(task);

        if (SelectedProject != null)
        {
            ITask? taskToRemove = SelectedProject.Tasks.FirstOrDefault(t => t.Id == task.Id);
            if (taskToRemove != null)
                SelectedProject.Tasks.Remove(taskToRemove);
        }

        if (SelectedTask?.Id == task.Id)
            SelectedTask = null;

        _logger.Log(LogLevel.INFO, $"Removed Task From Board: {task.Id}");
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

            // Move only after the API accepts the update so the board does not drift from the database.
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
        // Centralize progress-to-column mapping so drag/drop, create, update, and delete stay consistent.
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

        // SelectedProject.Tasks is the source used when reselecting the project, so keep it current too.
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
        _logger.Log(LogLevel.INFO, $"Added Created Project To Project List: {project.Id}");
    }

    private async Task GetProjectsAsync()
    {
        try
        {
            // The API returns only projects associated with the active user, including every task in those projects.
            var projects = await _projectService.GetProjectsAsync(_userService.Id);

            ListOfProjects.Clear();
            foreach (var project in projects)
            {
                if (!project.IsArchived)
                {
                    ListOfProjects.Add(project);
                }
            }
            _logger.Log(LogLevel.INFO, "All Projects Fetched Succesfully.");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Load Projects: {ex.Message}");
        }
    }

    public async Task CreateProjectAsync(Project project)
    {
        try
        {
            Project savedProject = await _projectService.CreateProjectAsync(project, _userService.Id);
            AddCreatedProject(savedProject);
            _logger.Log(LogLevel.INFO, $"Created Project {savedProject.Id}: {savedProject.Title}");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Create Project: {ex.Message}");
        }
    }

    private async void OnGlobalStateChange(object? sender, EventArgs e)
    {
        if (_isUpdatingGlobalState) return;
        _isUpdatingGlobalState = true;

        try
        {
            int previousIndex = SelectedProject != null ? ListOfProjects.IndexOf(SelectedProject) : -1;

            await GetProjectsAsync();

            if (previousIndex >= 0 && previousIndex < ListOfProjects.Count)
            {
                SelectedProject = ListOfProjects[previousIndex];
            }
            else
            {
                SelectedProject = null;
                ClearTaskColumns();
            }
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Error processing global state sync: {ex.Message}");
        }
        finally
        {
            _isUpdatingGlobalState = false;
        }
    }

    public void Dispose()
    {
        _taskService.TasksChanged -= OnGlobalStateChange;
        _projectService.ProjectsChanged -= OnGlobalStateChange;
        GC.SuppressFinalize(this);
    }
}
