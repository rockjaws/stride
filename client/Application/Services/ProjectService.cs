using System.Net.Http;
using System.Net.Http.Json;
using client.Application.Interfaces;
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
    return await _httpclient.GetFromJsonAsync<List<Project>>("api/projects") ?? new List<Project>();
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
}
