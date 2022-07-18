using Auth.Authentication;
using Auth.Authentication.Constants;
using Auth.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auth.WebApi.Controllers;

[ApiController]
[Route("oauth/tokens")]
public class TokenController : ControllerBase
{
    private readonly AuthenticationService _authenticationService;
    private readonly UserManager<User> _userManager;

    public TokenController(AuthenticationService authenticationService,
        UserManager<User> userManager)
    {
        _authenticationService = authenticationService;
        _userManager = userManager;
    }
    
    [HttpPost]
    public async Task<IActionResult> SignInAsync(AccessTokenRequest request) //SignInCommand command)
    {
        async Task<User> ValidateAsync(string grantType, long userId)
        {
            switch (grantType)
            {
                case GrantTypes.Password:
                {
                    var user = await _userManager.FindByEmailAsync(request.Username);
                    if (user == null)
                        throw new InvalidOperationException("Invalid username or password");

                    var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
                    if (!validPassword)
                        throw new InvalidOperationException("Invalid username or password");

                    return user;
                }
                case GrantTypes.RefreshToken:
                    return await _userManager.FindByIdAsync(userId.ToString());
                default:
                    throw new NotSupportedException();
            }
        }

        var value = await _authenticationService.CreateTokensAsync(request, ValidateAsync, 
            string.Empty, "127.0.0.1", string.Empty);

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