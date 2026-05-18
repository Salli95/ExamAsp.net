using IdentityService.Domain;
using IdentityService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IdentityDbContext _context;

    public AuthController(IdentityDbContext context)
    {
        _context = context;
    }

    // 1. Эндпоинт Регистрации: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto request)
    {
        // Проверяем, нет ли уже пользователя с таким Email
        var userExists = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (userExists)
            return BadRequest(new { message = "Пользователь с таким Email уже существует!" });

        // В реальном проекте здесь должен быть хэш пароля (BCrypt или Identity), 
        // но для экзамена и простоты пока сохраняем строку напрямую
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = request.Password, // Временно пишем напрямую
            Role = "User"
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Пользователь успешно зарегистрирован!" });
    }

    // 2. Эндпоинт Входа: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        
        if (user == null || user.PasswordHash != request.Password)
            return Unauthorized(new { message = "Неверный Email или пароль!" });

        // Возвращаем данные пользователя (позже прикрутим JWT-токен, если препод потребует)
        return Ok(new { 
            message = "Вход выполнен успешно!", 
            userId = user.Id, 
            email = user.Email, 
            role = user.Role 
        });
    }
}

// Вспомогательный класс (DTO) для приема данных с запроса
public class UserDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}