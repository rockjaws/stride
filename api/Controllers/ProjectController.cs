using api.DTOs;
using api.Models;
using api.Repositories;
using api.Extensions;

using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectController : ControllerBase
{
    private readonly IProjectRepository _repository;
    private readonly IChannelRepository _channelRepository;
    private readonly IUserRepository _userRepository;

    public ProjectController(IProjectRepository repository, IChannelRepository channelRepository, IUserRepository userRepository)
    {
        _repository = repository;
        _channelRepository = channelRepository;
        _userRepository = userRepository;
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

        if (project == null)
            return NotFound();

        project.IsArchived = dto.IsArchived;

        await _repository.UpdateProjectAsync(project);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto?>> GetProject(int id)
    {
        var project = await _repository.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }
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

        var generalChannel = new ChatChannel
        {
            Name = "general",
            ProjectId = project.Id
        };

        await _channelRepository.CreateChannelAsync(generalChannel);
        await _channelRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project.ToDto());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProject(int id, ProjectUpdateDto dto)
    {
        var project = await _repository.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        project.Title = dto.Title;
        project.Description = dto.Description;
        project.Deadline = dto.Deadline;

        await _repository.UpdateProjectAsync(project);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProject(int id)
    {
        var project = await _repository.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        await _repository.DeleteProjectAsync(project);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/channels/{channelId}")]
    public async Task<ActionResult<ChannelDto>> GetChannel(int id, int channelId)
    {
        var channel = await _channelRepository.GetChannelByIdAsync(channelId);
        if (channel == null)
        {
            return NotFound();
        }
        return Ok(
            new ChannelDto
            {
                Id = channel.Id,
                Name = channel.Name,
                ProjectId = channel.ProjectId,
            }
        );
    }

    [HttpPost("{id}/channels")]
    public async Task<ActionResult> CreateNewChannel(int id, ChannelCreateDto dto)
    {
        var project = await _repository.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        var chatChannel = new ChatChannel { Name = dto.Name, ProjectId = id };

        await _channelRepository.CreateChannelAsync(chatChannel);
        await _channelRepository.SaveChangesAsync();

        var chatChannelDto = new ChannelDto
        {
            Id = chatChannel.Id,
            Name = chatChannel.Name,
            ProjectId = chatChannel.ProjectId,
        };

        return CreatedAtAction(
            nameof(GetChannel),
            new { id, channelId = chatChannel.Id },
            chatChannelDto
        );
    }
}
