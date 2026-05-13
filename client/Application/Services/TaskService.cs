using System.Net.Http;
using System.Net.Http.Json;
using client.Application.Interfaces;
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
    return await _httpclient.GetFromJsonAsync<List<ProjectTask>>("api/tasks") ?? new List<ProjectTask>();
  }

  public async Task CreateTaskAsync(ProjectTask task)
  {
    var response = await _httpclient.PostAsJsonAsync("api/tasks", task);
    response.EnsureSuccessStatusCode();
  }

  public async Task UpdateTaskAsync(ProjectTask task)
  {
    var response = await _httpclient.PutAsJsonAsync($"api/tasks/{task.Id}", task);
    response.EnsureSuccessStatusCode();
  }

  public async Task DeleteTaskAsync(int id)
  {
    var response = await _httpclient.DeleteAsync($"api/tasks/{id}");
    response.EnsureSuccessStatusCode();
  }

  public void SortTasks() { }
}
