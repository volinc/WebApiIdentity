using Authentication;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiIdentity.Domain;
using WebApiIdentity.Domain.Identity;

namespace WebApiIdentity.Application.Token.Commands.SignIn;

public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInCommandResult>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly JwtSettings _jwtSettings;
    private readonly CurrentTimeFunc _now;

    public SignInCommandHandler(UserManager<User> userManager, 
        SignInManager<User> signInManager,
        JwtSecurityTokenHandler jwtSecurityTokenHandler,
        JwtSettings jwtSettings,
        CurrentTimeFunc now)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        _jwtSettings = jwtSettings;
        _now = now;
    }
    
    public async Task<SignInCommandResult> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        return request.GrantType switch
        {
            GrantTypes.Password => await SingInPasswordAsync(request),
            _ => throw new NotSupportedException()
        };
    }

    // https://devblogs.microsoft.com/dotnet/bearer-token-authentication-in-asp-net-core/
    private async Task<SignInCommandResult> SingInPasswordAsync(SignInCommand request)
    {
        var user = await _userManager.FindByEmailAsync(request.Username);
        if (user == null)
            throw new InvalidOperationException("Invalid username or password");

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!signInResult.Succeeded)
            throw new InvalidOperationException("Invalid username or password");

        var signId = Guid.NewGuid().ToString("N");
        var accessToken = CreateAccessToken(signId, user);

        return new SignInCommandResult
        {
            AccessToken = _jwtSecurityTokenHandler.WriteToken(accessToken),
            ExpiresIn = (int)_jwtSettings.AccessTokenLifetime.TotalSeconds,
            TokenType = "bearer"
        };
    }

    private JwtSecurityToken CreateAccessToken(string signId, User user)
    {
        var tokenId = Guid.NewGuid().ToString("N");

        var subject = new ClaimsIdentity(new[]
        {
            new Claim(Constants.JwtClaimTypes.Subject, user.Id.ToString()),
            new Claim(Constants.JwtClaimTypes.Email, user.Email),
            new Claim(Constants.CustomClaimTypes.SignId, signId),
            new Claim(Constants.JwtClaimTypes.TokenId, tokenId),
            new Claim(Constants.CustomClaimTypes.TokenType, Constants.JwtTokenTypes.Access)
        });
        
        if (user.NormalizedUserName != null)
            subject.AddClaim(new Claim(Constants.JwtClaimTypes.Name, user.NormalizedUserName));
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.AccessTokenSecret));
        const string algorithm = SecurityAlgorithms.HmacSha256Signature;
        var signingCredentials = new SigningCredentials(key, algorithm);

        var currentTime = _now();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Audience = _jwtSettings.Audience,
            Issuer = _jwtSettings.Issuer,
            Expires = (currentTime + _jwtSettings.AccessTokenLifetime).DateTime.ToUniversalTime(),
            SigningCredentials = signingCredentials,
            IssuedAt = currentTime.DateTime.ToUniversalTime()
        };

        return _jwtSecurityTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
    }
}