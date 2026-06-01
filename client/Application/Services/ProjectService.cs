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
        _httpclient = new HttpClient { BaseAddress = new Uri("http://localhost:5189") };
    }

    public async Task<List<Project>> GetProjectsAsync()
    {
        var projectDtos =
            await _httpclient.GetFromJsonAsync<List<ProjectDto>>("api/projects") ?? [];
        return [.. projectDtos.Where(p => !p.IsArchived).Select(ToProject)];
    }

    public async Task<List<Project>> GetProjectsAsync(int userId)
    {
        // User membership filtering is handled by the API endpoint.
        var projectDtos =
            await _httpclient.GetFromJsonAsync<List<ProjectDto>>($"api/projects?userId={userId}")
            ?? [];

        return [.. projectDtos.Select(ToProject)];
    }

    public async Task<Project> CreateProjectAsync(Project project, int userId)
    {
        var response = await _httpclient.PostAsJsonAsync(
            "api/projects",
            new ProjectCreateDto
            {
                Title = project.Title,
                Description = project.Description,
                StartDate = project.StartDate,
                Deadline = project.Deadline,
                UserId = userId,
            }
        );
        response.EnsureSuccessStatusCode();

        var createdProject =
            await response.Content.ReadFromJsonAsync<ProjectDto>()
            ?? throw new InvalidOperationException("The API did not return the created project.");

        return ToProject(createdProject);
    }

    public async Task<ChatChannel> CreateChannelAsync(int projectId, string name)
    {
        var response = await _httpclient.PostAsJsonAsync(
            $"api/projects/{projectId}/channels",
            new { name }
        );
        response.EnsureSuccessStatusCode();

        var dto =
            await response.Content.ReadFromJsonAsync<ChannelDto>()
            ?? throw new InvalidOperationException("Api did not return new channel");

        return new ChatChannel(dto.Id, dto.Name, dto.ProjectId);
    }

    public async Task DeleteProjectAsync(int id)
    {
        var response = await _httpclient.DeleteAsync($"api/projects/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task SetProjectArchivedAsync(int id, bool isArchived)
    {
        var payload = new { isArchived };

        var response = await _httpclient.PatchAsJsonAsync($"api/projects/{id}/archive", payload);

        response.EnsureSuccessStatusCode();
    }

    private static TaskProgress ParseProgress(string progress)
    {
        if (Enum.TryParse<TaskProgress>(progress, ignoreCase: true, out var parsed))
            return parsed;

        return TaskProgress.Backlog;
    }

    private static TaskPriority ParsePriority(string priority)
    {
        if (Enum.TryParse<TaskPriority>(priority, ignoreCase: true, out var parsed))
            return parsed;

        return TaskPriority.Normal;
    }

    private static Project ToProject(ProjectDto dto)
    {
        var members = dto
            .Users.Select(u => new User(u.Id, u.FirstName, u.LastName, u.WorkMail))
            .ToList();

        return new Project(
            dto.Id,
            dto.Title,
            dto.Description,
            dto.StartDate,
            dto.Deadline,
            [.. dto.ChatChannels.Select(c => new ChatChannel(c.Id, c.Name, c.ProjectId))],
            dto.IsArchived,
            // Project tasks are nested in the project response so the kanban board can populate immediately.
            [
                .. dto.Tasks.Select(t => new ProjectTask(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.StartDate,
                    t.Deadline,
                    ParseProgress(t.Progress),
                    ParsePriority(t.Priority),
                    t.ProjectId
                )
                {
                    // ADD THIS EXACT LINE:
                    // This is what actually takes the JSON users and hands them to the checkboxes!
                    UsersAssigned = t
                        .Users.Select(u => new User(u.Id, u.FirstName, u.LastName, u.WorkMail))
                        .ToList(),
                }),
            ],
            members
        );
    }

    private sealed class ProjectCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public int UserId { get; set; }
    }

    private sealed class ProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsArchived { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public List<ProjectTaskDto> Tasks { get; set; } = [];
        public List<UserDto> Users { get; set; } = [];
        public List<ChannelDto> ChatChannels { get; set; } = [];
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

    private sealed class ChannelDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProjectId { get; set; }
    }
}
