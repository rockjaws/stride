using api.DTOs;
using api.Extensions;
using api.Models;
using api.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectController : ControllerBase
{
    private readonly IProjectRepository _repository;
    private readonly IChannelRepository _channelRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;

    public ProjectController(
        IProjectRepository repository,
        IChannelRepository channelRepository,
        IUserRepository userRepository,
        INotificationRepository notificationRepository
    )
    {
        _repository = repository;
        _channelRepository = channelRepository;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects([FromQuery] int? userId)
    {
        var projects = userId.HasValue
            ? await _repository.GetProjectsByUserIdAsync(userId.Value)
            : await _repository.GetAllProjectsAsync();

        var dtos = projects.Select(p => p.ToDto());
        return Ok(dtos);
    }

    [HttpPatch("{id}/archive")]
    public async Task<ActionResult> SetArchived(int id, ProjectArchiveDto dto)
    {
        var project = await _repository.GetProjectByIdAsync(id);
        if (project == null) return NotFound();

        project.IsArchived = dto.IsArchived;
        await _repository.UpdateProjectAsync(project);
        await _repository.SaveChangesAsync();

        string actionText = dto.IsArchived ? $"Project '{project.Title}' was archived." : $"Project '{project.Title}' was restored.";
        await LogProjectActivityAsync(id, actionText);

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto?>> GetProject(int id)
    {
        var project = await _repository.GetProjectByIdAsync(id);
        if (project == null) return NotFound();
        return Ok(project.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> CreateProject(ProjectCreateDto dto)
    {
        var project = new Project
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            Deadline = dto.Deadline,
        };

        var user = await _userRepository.GetUserByIdAsync(dto.UserId);
        if (user != null)
        {
            project.Users.Add(user);
        }

        await _repository.AddProjectAsync(project);
        await _repository.SaveChangesAsync();

        var generalChannel = new ChatChannel { Name = "general", ProjectId = project.Id };
        await _channelRepository.CreateChannelAsync(generalChannel);
        await _channelRepository.SaveChangesAsync();

        await LogProjectActivityAsync(project.Id, $"Project '{project.Title}' was created.");

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project.ToDto());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProject(int id, ProjectUpdateDto dto)
    {
        var project = await _repository.GetProjectByIdAsync(id);
        if (project == null) return NotFound();

        project.Title = dto.Title;
        project.Description = dto.Description;
        project.Deadline = dto.Deadline;

        await _repository.UpdateProjectAsync(project);
        await _repository.SaveChangesAsync();

        await LogProjectActivityAsync(id, $"Project details for '{project.Title}' were updated.");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProject(int id)
    {
        var project = await _repository.GetProjectByIdAsync(id);
        if (project == null) return NotFound();

        await _repository.DeleteProjectAsync(project);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/channels/{channelId}")]
    public async Task<ActionResult<ChannelDto>> GetChannel(int id, int channelId)
    {
        var channel = await _channelRepository.GetChannelByIdAsync(channelId);
        if (channel == null) return NotFound();

        return Ok(new ChannelDto
        {
            Id = channel.Id,
            Name = channel.Name,
            ProjectId = channel.ProjectId,
        });
    }

    [HttpDelete("{projectId}/channels/{channelId}")]
    public async Task<ActionResult> DeleteChannel(int projectId, int channelId)
    {
        var channel = await _channelRepository.GetChannelByIdAsync(channelId);
        if (channel == null || channel.ProjectId != projectId) return NotFound();

        string channelName = channel.Name;
        await _channelRepository.DeleteChannelAsync(channel);
        await _channelRepository.SaveChangesAsync();

        await LogProjectActivityAsync(projectId, $"Channel #{channelName} was deleted.");

        return NoContent();
    }

    [HttpPost("{id}/channels")]
    public async Task<ActionResult> CreateNewChannel(int id, ChannelCreateDto dto)
    {
        var project = await _repository.GetProjectByIdAsync(id);
        if (project == null) return NotFound();

        var chatChannel = new ChatChannel { Name = dto.Name, ProjectId = id };
        await _channelRepository.CreateChannelAsync(chatChannel);
        await _channelRepository.SaveChangesAsync();

        await LogProjectActivityAsync(id, $"New channel #{chatChannel.Name} was created.");

        var chatChannelDto = new ChannelDto
        {
            Id = chatChannel.Id,
            Name = chatChannel.Name,
            ProjectId = chatChannel.ProjectId,
        };

        return CreatedAtAction(nameof(GetChannel), new { id, channelId = chatChannel.Id }, chatChannelDto);
    }

    [HttpGet("{id}/notifications")]
    public async Task<ActionResult> GetProjectNotificatons(int id, [FromQuery] int? userId)
    {
        var notifications = await _notificationRepository.GetNotificationsByProjectIdAsync(id);

        if (userId.HasValue)
        {
            notifications = notifications.Where(n => n.UserId == userId.Value);
        }

        return Ok(notifications.Select(n => n.ToDto()));
    }

    private async Task LogProjectActivityAsync(int projectId, string text)
    {
        var project = await _repository.GetProjectByIdAsync(projectId);
        if (project == null) return;

        foreach (var user in project.Users)
        {
            await _notificationRepository.AddNotificationAsync(new Notification
            {
                Text = text,
                ProjectId = projectId,
                UserId = user.Id,
                Time = DateTime.Now,
                IsRead = true
            });
        }
        await _notificationRepository.SaveChangesAsync();
    }
}
