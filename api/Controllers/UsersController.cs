using api.DTOs;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly INotificationRepository _notificationRepository;

    public UsersController(
        IUserRepository repository,
        INotificationRepository notificationRepository
    )
    {
        _repository = repository;
        _notificationRepository = notificationRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] int? projectId)
    {
        var users = projectId.HasValue
            ? await _repository.GetUsersByProjectIdAsync(projectId.Value)
            : await _repository.GetAllUsersAsync();

        var dtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            WorkMail = u.WorkMail,
        });
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto?>> GetUser(int id)
    {
        var user = await _repository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var dto = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            WorkMail = user.WorkMail,
        };
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(UserCreateDto dto)
    {
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            WorkMail = dto.WorkMail,
        };

        await _repository.AddUserAsync(user);
        await _repository.SaveChangesAsync();

        var userDto = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            WorkMail = user.WorkMail,
        };

        return CreatedAtAction(nameof(GetUser), new { id = userDto.Id }, userDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(int id, UserUpdateDto dto)
    {
        var user = await _repository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.WorkMail = dto.WorkMail;

        await _repository.UpdateUserAsync(user);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/notifications")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications(int id)
    {
        var user = await _repository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var notifications = await _notificationRepository.GetNotificationsByIdAsync(id);
        if (notifications == null)
        {
            return NotFound();
        }

        // Notifications are scoped by route user id so each client only polls its own feed.
        var dtos = notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Text = n.Text,
            IsRead = n.IsRead,
            Time = n.Time,
            TaskId = n.TaskId,
        });
        return Ok(dtos);
    }

    [HttpPut("{id}/notifications/{notificationId}")]
    public async Task<ActionResult> UpdateNotification(
        int id,
        int notificationId,
        NotificationUpdateDto dto
    )
    {
        var user = await _repository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var notification = await _notificationRepository.GetNotificationByIdAsync(notificationId);
        if (notification == null)
        {
            return NotFound();
        }

        notification.IsRead = dto.IsRead;
        await _notificationRepository.UpdateNotification(notification);
        await _notificationRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var user = await _repository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        await _repository.DeleteUserAsync(user);
        await _repository.SaveChangesAsync();

        return NoContent();
    }
}
