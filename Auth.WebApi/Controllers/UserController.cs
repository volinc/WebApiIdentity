using Auth.Abstractions;
using Auth.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auth.WebApi.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly CurrentTimeFunc _now;

    public UserController(UserManager<User> userManager,
        CurrentTimeFunc now)
    {
        _userManager = userManager;
        _now = now;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUserAsync()
    {
        var userName = Guid.NewGuid().ToString("N").ToLowerInvariant();
        var email = $"{userName}@go.offline";
        var createdAt = _now();

        var user = new User
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true,
            UpdatedAt = createdAt,
            CreatedAt = createdAt
        };
        var passwordHash = _userManager.PasswordHasher.HashPassword(user, "qwerty123");
        user.PasswordHash = passwordHash;
        
        var resultValue = await _userManager.CreateAsync(user);
        return Ok(resultValue);
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var user = User;
        
        await Task.Delay(0);
        return Ok(string.Join(Environment.NewLine, user.Claims.Select(x => x.Value)));
    }
}