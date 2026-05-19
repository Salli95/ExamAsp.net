using IdentityService.Api.Services;
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
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public AuthController(IdentityDbContext context, JwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { message = "Email обязателен." });

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Пароль обязателен." });

        var userExists = await _context.Users.AnyAsync(u => u.Email == request.Email);

        if (userExists)
            return BadRequest(new { message = "Пользователь с таким Email уже существует!" });

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = "User"
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Пользователь успешно зарегистрирован!",
            userId = newUser.Id,
            email = newUser.Email,
            role = newUser.Role
        });
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { message = "Email обязателен." });

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Пароль обязателен." });

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Неверный Email или пароль!" });

        var token = _jwtTokenGenerator.Generate(user);

        return Ok(new
        {
            message = "Вход выполнен успешно!",
            token,
            userId = user.Id,
            email = user.Email,
            role = user.Role
        });
    }
}

public class UserDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}