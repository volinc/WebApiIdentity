using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApiIdentity.Domain.Identity;

namespace WebApiIdentity.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public UserController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserAsync()
    {
        await Task.Delay(0);
        return Ok();
    }
}