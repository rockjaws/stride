using api.DTOs;
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

    public ProjectTasksController(ITaskRepository repository, INotificationRepository notificationRepository)
    {
        _repository = repository;
        _notificationRepository = notificationRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectTaskDto>>> GetProjectTasks([FromQuery] int? userId)
    {
        // When a user id is supplied, only return tasks assigned directly to that user.
        var projectTasks = userId is int id
          ? await _repository.GetTasksByUserIdAsync(id)
          : await _repository.GetAllTasksAsync();

        var dtos = projectTasks.Select(t => new ProjectTaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            StartDate = t.StartDate,
            Deadline = t.Deadline,
            Progress = t.Progress,
            Priority = t.Priority,
            ProjectId = t.ProjectId,
            Users = [.. t.Users.Select(u => new UserDto {
           Id = u.Id,
           FirstName = u.FirstName,
           LastName = u.LastName,
           WorkMail = u.WorkMail,
           })]
        });
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

        var dtos = new ProjectTaskDto
        {
            Id = projectTask.Id,
            Title = projectTask.Title,
            Description = projectTask.Description,
            StartDate = projectTask.StartDate,
            Deadline = projectTask.Deadline,
            Progress = projectTask.Progress,
            Priority = projectTask.Priority,
            ProjectId = projectTask.ProjectId,
            Users = [.. projectTask.Users.Select(u => new UserDto {
           Id = u.Id,
           FirstName = u.FirstName,
           LastName = u.LastName,
           WorkMail = u.WorkMail,
           })]
        };

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

        var projectTaskDto = new ProjectTaskDto
        {
            Id = projectTask.Id,
            Title = projectTask.Title,
            Description = projectTask.Description,
            StartDate = projectTask.StartDate,
            Deadline = projectTask.Deadline,
            Progress = projectTask.Progress,
            Priority = projectTask.Priority,
            ProjectId = projectTask.ProjectId,
            Users = []
        };

        return CreatedAtAction(nameof(GetProjectTask), new { id = projectTaskDto.Id }, projectTaskDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, ProjectTaskUpdateDto dto)
    {
        var projectTask = await _repository.GetTaskByIdAsync(id);
        if (projectTask == null)
        {
            return NotFound();
        }

        projectTask.Title = dto.Title;
        projectTask.Description = dto.Description;
        projectTask.StartDate = dto.StartDate;
        projectTask.Deadline = dto.Deadline;
        projectTask.Progress = dto.Progress;
        projectTask.Priority = dto.Priority;

        await _repository.UpdateTaskAsync(projectTask);

        // Notify every assigned user in the same unit of work as the task update.
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

        await _repository.DeleteTaskAsync(projectTask);
        await _repository.SaveChangesAsync();

        return NoContent();
    }
}
