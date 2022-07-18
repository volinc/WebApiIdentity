using Auth.Abstractions;
using Auth.Constants;
using Auth.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auth.WebApi.Controllers;

[ApiController]
[Route("users")]
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
        var email = $"{userName}@gmail.com";
        var createdAt = _now();

        var user = new User
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true,
            UpdatedAt = createdAt,
            CreatedAt = createdAt
        };
        var identityResult = await _userManager.CreateAsync(user, "qwerty123");
        if (identityResult.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, Roles.Customer);
        }
        
        return Ok(identityResult);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Customer)]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var user = User;

        var isAdmin = user.IsInRole(Roles.Admin);
        
        await Task.Delay(0);
        return Ok(string.Join(Environment.NewLine, user.Claims.Select(x => $"{x.Type} = {x.Value}")));
    }
}