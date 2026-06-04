using api.DTOs;
using api.Extensions;
using api.Models;
using api.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/tasks")]
public class ProjectTasksController : ControllerBase
{
    private readonly ITaskRepository _repository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;

    public ProjectTasksController(
        ITaskRepository repository,
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        IProjectRepository projectRepository
    )
    {
        _repository = repository;
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectTaskDto>>> GetProjectTasks([FromQuery] int? userId)
    {
        // When a user id is supplied, only return tasks assigned directly to that user.
        var projectTasks = userId is int id
          ? await _repository.GetTasksByUserIdAsync(id)
          : await _repository.GetAllTasksAsync();

        var dtos = projectTasks.Select(t => t.ToDto());
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectTaskDto?>> GetProjectTask(int id)
    {
        var projectTask = await _repository.GetTaskByIdAsync(id);
        if (projectTask == null)
        {
            return NotFound();
        }

        var dtos = projectTask.ToDto();
        return Ok(dtos);
    }

    [HttpPost]
    public async Task<ActionResult> CreateTask(ProjectTaskCreateDto dto)
    {
        var projectTask = new ProjectTask
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            Deadline = dto.Deadline,
            Priority = dto.Priority,
            ProjectId = dto.ProjectId
        };
        await _repository.AddTaskAsync(projectTask);
        await _repository.SaveChangesAsync();

        await LogProjectActivityAsync(projectTask.ProjectId, $"Task '{projectTask.Title}' was added to the backlog.", projectTask.Id);

        return CreatedAtAction(nameof(GetProjectTask), new { id = projectTask.Id }, projectTask.ToDto());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, ProjectTaskUpdateDto dto)
    {
        var projectTask = await _repository.GetTaskByIdAsync(id);
        if (projectTask == null)
        {
            return NotFound();
        }

        // Track state before applying mutations to detect changes cleanly
        var oldProgress = projectTask.Progress;
        var oldPriority = projectTask.Priority;

        projectTask.Title = dto.Title;
        projectTask.Description = dto.Description;
        projectTask.StartDate = dto.StartDate;
        projectTask.Deadline = dto.Deadline;
        projectTask.Progress = dto.Progress;
        projectTask.Priority = dto.Priority;

        projectTask.Users.Clear();
        foreach (var userId in dto.AssignedUserIds)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user != null)
            {
                projectTask.Users.Add(user);
            }
        }

        await _repository.UpdateTaskAsync(projectTask);

        foreach (var user in projectTask.Users)
        {
            await _notificationRepository.AddNotificationAsync(new Notification
            {
                Text = $"Task updated: {projectTask.Title}",
                IsRead = false,
                Time = DateTime.Now,
                UserId = user.Id,
                TaskId = projectTask.Id
            });
        }

        if (oldProgress != projectTask.Progress)
        {
            await LogProjectActivityAsync(projectTask.ProjectId, $"Task '{projectTask.Title}' was moved to {projectTask.Progress}.", projectTask.Id);
        }

        if (oldPriority != projectTask.Priority)
        {
            await LogProjectActivityAsync(projectTask.ProjectId, $"Task '{projectTask.Title}' priority was changed to {projectTask.Priority}.", projectTask.Id);
        }
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var projectTask = await _repository.GetTaskByIdAsync(id);
        if (projectTask == null)
        {
            return NotFound();
        }

        int projectId = projectTask.ProjectId;
        string taskTitle = projectTask.Title;

        await _repository.DeleteTaskAsync(projectTask);
        await _repository.SaveChangesAsync();

        await LogProjectActivityAsync(projectId, $"Task '{taskTitle}' was removed.", null);

        return NoContent();
    }

    private async Task LogProjectActivityAsync(int projectId, string text, int? taskId)
    {
        var project = await _projectRepository.GetProjectByIdAsync(projectId);
        if (project == null || project.Users == null) return;

        foreach (var user in project.Users)
        {
            await _notificationRepository.AddNotificationAsync(new Notification
            {
                Text = text,
                ProjectId = projectId,
                UserId = user.Id,
                TaskId = taskId,
                Time = DateTime.Now,
                IsRead = true // Your hack: true skips toast alerts but populates the persistent feed timelines!
            });
        }
        await _notificationRepository.SaveChangesAsync();
    }
}
