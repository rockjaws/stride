using System.Net.Http;
using System.Net.Http.Json;
using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;

namespace client.Application.Services;

public class ProjectService : IProjectService
{
  private readonly HttpClient _httpclient;

  public ProjectService()
  {
    _httpclient = new HttpClient
    {
      BaseAddress = new Uri("http://localhost:5189")
    };
  }

  public async Task<List<Project>> GetProjectsAsync()
  {
    var projectDtos = await _httpclient.GetFromJsonAsync<List<ProjectDto>>("api/projects") ?? [];

    return [.. projectDtos.Select(p => new Project(
      p.Id,
      p.Title,
      p.Description,
      p.StartDate,
      p.Deadline,
      [],
      [.. p.Tasks.Select(t => new ProjectTask(
        t.Id,
        t.Title,
        t.Description,
        t.StartDate,
        t.Deadline,
        ParseProgress(t.Progress),
        ParsePriority(t.Priority)
      ))]
    ))];
  }

  public async Task CreateProjectAsync(Project project)
  {
    var response = await _httpclient.PostAsJsonAsync("api/projects", project);
    response.EnsureSuccessStatusCode();
  }

  public async Task DeleteProjectAsync(int id)
  {
    var response = await _httpclient.DeleteAsync($"api/projects/{id}");
    response.EnsureSuccessStatusCode();
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

  private sealed class ProjectDto
  {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime Deadline { get; set; }
    public List<ProjectTaskDto> Tasks { get; set; } = [];
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
  }
}
