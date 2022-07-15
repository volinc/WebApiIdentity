using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using WebApiIdentity.Domain.Identity;
using WebApiIdentity.Models;

namespace WebApiIdentity.Application.Token.Commands.SignIn;

public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInCommandResult>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public SignInCommandHandler(UserManager<User> userManager, 
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    public async Task<SignInCommandResult> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        switch (request.GrantType)
        {   
            case GrantTypes.Password:
                return await SingInPasswordAsync(request);
            default:
                throw new NotSupportedException();
        }
    }

    // https://devblogs.microsoft.com/dotnet/bearer-token-authentication-in-asp-net-core/
    private async Task<SignInCommandResult> SingInPasswordAsync(SignInCommand request)
    {
        var user = await _userManager.FindByEmailAsync(request.Username);
        if (user == null)
            throw new InvalidOperationException("Invalid username or password");

        if (!await _signInManager.CanSignInAsync(user) || (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user)))
            throw new InvalidOperationException("The specified user cannot sign in");

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            throw new InvalidOperationException("Invalid username or password");

        if (_userManager.SupportsUserLockout)
            await _userManager.ResetAccessFailedCountAsync(user);

        var principal = await _signInManager.CreateUserPrincipalAsync(user);

        var ticket = new AuthenticationTicket(
            principal,
            new AuthenticationProperties(),
            JwtBearerDefaults.AuthenticationScheme);

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (signInResult.Succeeded)
        {

        }

        //var now = DateTimeOffset.UtcNow;
        //await _signInManager.SignInAsync(user, new OAuthChallengeProperties
        //{
        //    AllowRefresh = true,
        //    ExpiresUtc = now.AddMinutes(5),
        //    IsPersistent = true,
        //    IssuedUtc = now

        //}, request.GrantType);

        return new SignInCommandResult();
    }
}