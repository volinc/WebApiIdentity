using Auth.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Auth.WebApi.Controllers;

[ApiController]
[Route("oauth/tokens")]
public class TokenController : ControllerBase
{
    private readonly AuthenticationService _authenticationService;
    
    public TokenController(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    
    [HttpPost]
    public async Task<IActionResult> SignInAsync(AccessTokenRequest request)
    {
        var value = await _authenticationService.AuthenticateAsync(request);

        return Ok(value);
    }
    
    [HttpDelete]
    public async Task<IActionResult> SignOutAsync()
    {
        await Task.Delay(0);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        await Task.Delay(0);
        return Ok();
    }
}