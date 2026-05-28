using System.Net.Http;
using System.Net.Http.Json;

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;

namespace client.Application.Services;

public class TaskService : ITaskService
{
    private readonly HttpClient _httpclient;

    public TaskService()
    {
        _httpclient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5189")
        };
    }

    public async Task<List<ProjectTask>> GetTasksAsync()
    {
        var taskDtos = await _httpclient.GetFromJsonAsync<List<ProjectTaskDto>>("api/tasks") ?? [];
        return [.. taskDtos.Select(ToProjectTask)];
    }

    public async Task<List<ProjectTask>> GetTasksAsync(int userId)
    {
        // The API applies the user filter; the client only passes the active UserService id.
        var taskDtos = await _httpclient.GetFromJsonAsync<List<ProjectTaskDto>>($"api/tasks?userId={userId}") ?? [];
        return [.. taskDtos.Select(ToProjectTask)];
    }

    public async Task<ProjectTask> CreateTaskAsync(ProjectTask task)
    {
        if (task.ProjectId == null)
            throw new InvalidOperationException("Cannot create a task without a selected project.");

        var createResponse = await _httpclient.PostAsJsonAsync("api/tasks", new ProjectTaskCreateDto
        {
            Title = task.Title,
            Description = task.Description,
            StartDate = task.StartDate,
            Deadline = task.Deadline,
            Priority = task.Priority.ToString(),
            ProjectId = task.ProjectId.Value
        });
        createResponse.EnsureSuccessStatusCode();

        var createdTask = await createResponse.Content.ReadFromJsonAsync<ProjectTaskDto>()
          ?? throw new InvalidOperationException("The API did not return the created task.");

        var savedTask = new ProjectTask(
          createdTask.Id,
          task.Title,
          task.Description,
          task.StartDate,
          task.Deadline,
          task.Progress,
          task.Priority,
          task.ProjectId
        );

        // The create endpoint does not currently accept progress, so persist the full task immediately after creation.
        await UpdateTaskAsync(savedTask);
        return savedTask;
    }

    public async Task UpdateTaskAsync(ProjectTask task)
    {
        if (task.Id == null)
            throw new InvalidOperationException("Cannot update a task before it has been saved.");

        // Send enum names because the API serializes task enums as strings.
        var response = await _httpclient.PutAsJsonAsync($"api/tasks/{task.Id}", new ProjectTaskUpdateDto
        {
            Title = task.Title,
            Description = task.Description,
            StartDate = task.StartDate,
            Deadline = task.Deadline,
            Progress = task.Progress.ToString(),
            Priority = task.Priority.ToString(),
            AssignedUserIds = task.UsersAssigned?.Select(u => u.Id).ToList() ?? []
        });
        response.EnsureSuccessStatusCode();
    }

    public async Task<ProjectTask> MoveTaskAsync(ProjectTask task, TaskProgress progress)
    {
        // Return a new instance so callers can replace bound collection items cleanly.
        var movedTask = new ProjectTask(
          task.Id,
          task.Title,
          task.Description,
          task.StartDate,
          task.Deadline,
          progress,
          task.Priority,
          task.ProjectId
        );

        await UpdateTaskAsync(movedTask);
        return movedTask;
    }

    public async Task DeleteTaskAsync(int id)
    {
        var response = await _httpclient.DeleteAsync($"api/tasks/{id}");
        response.EnsureSuccessStatusCode();
    }

    public void SortTasks() { }

    private static ProjectTask ToProjectTask(ProjectTaskDto dto)
    {
        return new ProjectTask(
          dto.Id,
          dto.Title,
          dto.Description,
          dto.StartDate,
          dto.Deadline,
          ParseProgress(dto.Progress),
          ParsePriority(dto.Priority),
          dto.ProjectId
        )
        {
            // ADD THIS HERE TOO:
            UsersAssigned = dto.Users.Select(u => new User(u.Id, u.FirstName, u.LastName, u.WorkMail)).ToList()
        };
    }

    private static TaskProgress ParseProgress(string progress)
    {
        if (Enum.TryParse<TaskProgress>(progress, ignoreCase: true, out var parsed))
            return parsed;

        // Keep the client usable if the API sends an old or unknown enum value.
        return TaskProgress.Backlog;
    }

    private static TaskPriority ParsePriority(string priority)
    {
        if (Enum.TryParse<TaskPriority>(priority, ignoreCase: true, out var parsed))
            return parsed;

        // Unknown priority should not block task rendering.
        return TaskPriority.Normal;
    }

    private sealed class ProjectTaskCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; } = "Normal";
        public int ProjectId { get; set; }
    }

    private sealed class ProjectTaskUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Progress { get; set; } = "Backlog";
        public string Priority { get; set; } = "Normal";
        public List<int> AssignedUserIds { get; set; } = [];
    }

    private sealed class ProjectTaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Progress { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public int? ProjectId { get; set; }
        public List<UserDto> Users { get; set; } = [];
    }

    private sealed class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string WorkMail { get; set; } = string.Empty;
    }
}
