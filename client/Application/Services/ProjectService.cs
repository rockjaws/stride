// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using System.Net.Http;
using System.Net.Http.Json;

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;

namespace client.Application.Services;

public class ProjectService : IProjectService
{
    private readonly HttpClient _httpclient;

    public event EventHandler? ProjectsChanged;

    // Author: Nicolaj and Oliver
    public ProjectService(HttpClient httpClient)
    {
        _httpclient = httpClient;
    }

    // Author: Nicolaj and Oliver
    public async Task<List<Project>> GetProjectsAsync()
    {
        var projectDtos =
            await _httpclient.GetFromJsonAsync<List<ProjectDto>>("api/projects") ?? [];
        return [.. projectDtos.Where(p => !p.IsArchived).Select(ToProject)];
    }

    // Author: Oliver
    public async Task<List<Project>> GetProjectsAsync(int userId)
    {
        // User membership filtering is handled by the API endpoint.
        var projectDtos =
            await _httpclient.GetFromJsonAsync<List<ProjectDto>>($"api/projects?userId={userId}")
            ?? [];

        return [.. projectDtos.Select(ToProject)];
    }

    // Author: Nicolaj and Oliver
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
                UserIds = project.Members.Select(u => u.Id).ToList(),
            }
        );
        response.EnsureSuccessStatusCode();
        NotifyProjectsChanged();

        var createdProject =
            await response.Content.ReadFromJsonAsync<ProjectDto>()
            ?? throw new InvalidOperationException("The API did not return the created project.");

        return ToProject(createdProject);
    }

    // Author: Oliver
    public async Task<Project> UpdateProjectAsync(Project project)
    {
        if (project.Id == null)
            throw new InvalidOperationException("Cannot update a project before it has been saved.");

        var response = await _httpclient.PutAsJsonAsync(
            $"api/projects/{project.Id}",
            new ProjectUpdateDto
            {
                Title = project.Title,
                Description = project.Description,
                StartDate = project.StartDate,
                Deadline = project.Deadline,
                UserIds = project.Members.Select(u => u.Id).ToList(),
            }
        );
        response.EnsureSuccessStatusCode();

        var updatedProject =
            await _httpclient.GetFromJsonAsync<ProjectDto>($"api/projects/{project.Id}")
            ?? throw new InvalidOperationException("The API did not return the updated project.");

        NotifyProjectsChanged();
        return ToProject(updatedProject);
    }

    // Author: Nicolaj and Oliver
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

    // Author: Oliver
    public async Task DeleteChannelAsync(int projectId, int channelId)
    {
        var response = await _httpclient.DeleteAsync(
            $"api/projects/{projectId}/channels/{channelId}"
        );
        response.EnsureSuccessStatusCode();
    }

    // Author: Nicolaj and Oliver
    public async Task DeleteProjectAsync(int id)
    {
        var response = await _httpclient.DeleteAsync($"api/projects/{id}");
        response.EnsureSuccessStatusCode();
        NotifyProjectsChanged();
    }

    // Author: Nicolaj and Oliver
    public async Task SetProjectArchivedAsync(int id, bool isArchived)
    {
        var payload = new { isArchived };

        var response = await _httpclient.PatchAsJsonAsync($"api/projects/{id}/archive", payload);

        response.EnsureSuccessStatusCode();
        NotifyProjectsChanged();
    }

    // Author: Nicolaj and Oliver
    private static TaskProgress ParseProgress(string progress)
    {
        if (Enum.TryParse<TaskProgress>(progress, ignoreCase: true, out var parsed))
            return parsed;

        return TaskProgress.Backlog;
    }

    // Author: Nicolaj and Oliver
    private static TaskPriority ParsePriority(string priority)
    {
        if (Enum.TryParse<TaskPriority>(priority, ignoreCase: true, out var parsed))
            return parsed;

        return TaskPriority.Normal;
    }

    // Author: Nicolaj and Oliver
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

    // Author: Nicolaj
    private void NotifyProjectsChanged() => ProjectsChanged?.Invoke(this, EventArgs.Empty);

    private sealed class ProjectCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public int UserId { get; set; }
        public List<int> UserIds { get; set; } = [];
    }

    private sealed class ProjectUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public List<int> UserIds { get; set; } = [];
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
