using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Authentication.Constants;
using Auth.Authentication.Helpers;
using Auth.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Authentication.Handlers;

public class AccessTokenHandler
{
    private readonly JwtSettings _jwtSettings;
    private readonly CurrentTimeFunc _now;

    public AccessTokenHandler(JwtSettings jwtSettings, 
        CurrentTimeFunc now)
    {
        _jwtSettings = jwtSettings;
        _now = now;
    }

    public JwtSecurityToken CreateToken(User user, IReadOnlyCollection<string> roles, string signId)
    {
        var tokenId = Guid.NewGuid().ToString("N");
        var userId = user.Id.ToString();

        var subject = new ClaimsIdentity(new[]
        {
            new Claim(CustomClaimTypes.TokenType, TokenTypes.Access),
            new Claim(CustomClaimTypes.TokenId, tokenId),
            new Claim(CustomClaimTypes.SignId, signId),
            new Claim(CustomClaimTypes.NameId, userId)
        });
        
        foreach (var role in roles)
            subject.AddClaim(new Claim(CustomClaimTypes.Role, role));

        if (user.Email != null)
        {
            subject.AddClaim(new Claim(CustomClaimTypes.Email, user.Email));
            var emailConfirmed = user.EmailConfirmed.ToString();
            subject.AddClaim(new Claim(CustomClaimTypes.EmailVerified, emailConfirmed));
        }

        var currentTime = _now();

        var securityKey = SecurityKeyHelper.Create(_jwtSettings.TokenPassword);
        const string algorithm = SecurityAlgorithms.HmacSha256Signature;
        var signingCredentials = new SigningCredentials(securityKey, algorithm);

        var issuedAt = currentTime.DateTime.ToUniversalTime();
        var expires = (currentTime + _jwtSettings.AccessTokenLifetime).DateTime.ToUniversalTime();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Issuer = _jwtSettings.Issuer,
            IssuedAt = issuedAt,
            Audience = _jwtSettings.Audience,
            Expires = expires,
            SigningCredentials = signingCredentials
        };

        return new JwtSecurityTokenHandler().CreateJwtSecurityToken(tokenDescriptor);
    }
}