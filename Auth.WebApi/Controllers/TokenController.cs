using Auth.Authentication;
using Auth.Identity;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AuthenticationService = Auth.Authentication.AuthenticationService;

namespace Auth.WebApi.Controllers;

[ApiController]
[Route("oauth")]
public class TokenController : ControllerBase
{
    private readonly AuthenticationService _authenticationService;
    private readonly IConfiguration _configuration;
    private readonly IConfigurationSection _goolgeSettings;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public TokenController(AuthenticationService authenticationService,
        IConfiguration configuration,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _authenticationService = authenticationService;
        _configuration = configuration;
        _goolgeSettings = configuration.GetSection("GoogleAuthSettings");
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    [HttpPost("tokens")]
    public async Task<IActionResult> SignInAsync(AccessTokenRequest request)
    {
        var value = await _authenticationService.AuthenticateAsync(request);

        return Ok(value);
    }
    
    [HttpDelete("tokens")]
    public async Task<IActionResult> SignOutAsync()
    {
        await Task.Delay(0);
        return Ok();
    }

    [HttpGet("tokens")]
    public async Task<IActionResult> GetListAsync()
    {
        await Task.Delay(0);
        return Ok();
    }

    //[HttpPost("external")]
    //public async Task<IActionResult> ExternalSignInAsync(ExternalAuthDto externalAuth)
    //{
    //    var payload =  await VerifyGoogleTokenAsync(externalAuth);
    //    if (payload == null)
    //        return BadRequest("Invalid External Authentication.");

    //    var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);
    //    var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
    //    if (user == null)
    //    {
    //        user = await _userManager.FindByEmailAsync(payload.Email);
    //        if (user == null)
    //        {
    //            user = new User { Email = payload.Email, UserName = payload.Email };
    //            await _userManager.CreateAsync(user);
    //            //prepare and send an email for the email confirmation
    //            await _userManager.AddToRoleAsync(user, "Viewer");
    //            await _userManager.AddLoginAsync(user, info);
    //        }
    //        else
    //        {
    //            await _userManager.AddLoginAsync(user, info);
    //        }
    //    }

    //    var token = await _jwtHandler.GenerateToken(user);
    //    return Ok(new AuthResponseDto { Token = token, IsAuthSuccessful = true });
    //}

    //public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(ExternalAuthDto externalAuth)
    //{
    //    var settings = new GoogleJsonWebSignature.ValidationSettings
    //    {
    //        Audience = new List<string> {_goolgeSettings.GetSection("clientId").Value}
    //    };
    //    return await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
    //}
}

//public class ExternalAuthDto
//{
//    public string? Provider { get; set; }
//    public string? IdToken { get; set; }
//}