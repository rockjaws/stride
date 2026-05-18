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

  public UsersController(IUserRepository repository)
  {
    _repository = repository;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
  {
    var users = await _repository.GetAllUsersAsync();
    var dtos = users.Select(u => new UserDto
    {
      Id = u.Id,
      FirstName = u.FirstName,
      LastName = u.LastName,
      WorkMail = u.WorkMail,
      Role = u.Role
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
      Role = user.Role
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
      Role = user.Role,
    };

    return CreatedAtAction(nameof(GetUser), new { id = userDto.Id }, userDto);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateUser(int id, UserUpdateDto dto)
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


  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteUser(int id)
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
