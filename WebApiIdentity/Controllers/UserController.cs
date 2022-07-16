using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApiIdentity.Domain.Identity;

namespace WebApiIdentity.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public UserController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUserAsync()
    {
        await Task.Delay(0);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var user = User;
        
        await Task.Delay(0);
        return Ok(string.Join(Environment.NewLine, user.Claims.Select(x => x.Value)));
    }
}