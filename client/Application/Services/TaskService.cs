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

    await UpdateTaskAsync(savedTask);
    return savedTask;
  }

  public async Task UpdateTaskAsync(ProjectTask task)
  {
    if (task.Id == null)
      throw new InvalidOperationException("Cannot update a task before it has been saved.");

    var response = await _httpclient.PutAsJsonAsync($"api/tasks/{task.Id}", new ProjectTaskUpdateDto
    {
      Title = task.Title,
      Description = task.Description,
      StartDate = task.StartDate,
      Deadline = task.Deadline,
      Progress = task.Progress.ToString(),
      Priority = task.Priority.ToString()
    });
    response.EnsureSuccessStatusCode();
  }

  public async Task<ProjectTask> MoveTaskAsync(ProjectTask task, TaskProgress progress)
  {
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
    );
  }

  private static TaskProgress ParseProgress(string progress)
  {
    if (Enum.TryParse<TaskProgress>(progress, ignoreCase: true, out var parsed))
      return parsed;

    return TaskProgress.BackLog;
  }

  private static TaskPriority ParsePriority(string priority)
  {
    if (Enum.TryParse<TaskPriority>(priority, ignoreCase: true, out var parsed))
      return parsed;

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
    public string Progress { get; set; } = "BackLog";
    public string Priority { get; set; } = "Normal";
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
  }
}
