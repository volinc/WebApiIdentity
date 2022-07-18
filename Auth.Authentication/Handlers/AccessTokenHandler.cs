using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Authentication.Constants;
using Auth.Authentication.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Authentication.Handlers;

public class AccessTokenHandler
{
    private readonly JwtSettings _jwtSettings;
    private readonly CurrentTimeFunc _now;

    public AccessTokenHandler(JwtSettings jwtSettings, CurrentTimeFunc now)
    {
        _jwtSettings = jwtSettings;
        _now = now;
    }

    public JwtSecurityToken CreateToken(string signId, IdentityUser<long> user)
    {
        var tokenId = Guid.NewGuid().ToString("N");

        var subject = new ClaimsIdentity(new[]
        {
            new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
            new Claim(CustomClaimTypes.SignId, signId),
            new Claim(JwtClaimTypes.TokenId, tokenId),
            new Claim(CustomClaimTypes.TokenType, JwtTokenTypes.Access)
        });

        if (user.UserName != null)
            subject.AddClaim(new Claim(JwtClaimTypes.Name, user.UserName));

        if (user.Email != null)
            subject.AddClaim(new Claim(JwtClaimTypes.Email, user.Email));

        var currentTime = _now();

        var securityKey = SecurityKeyHelper.Create(_jwtSettings.TokenPassword);
        const string algorithm = SecurityAlgorithms.HmacSha256Signature;
        var signingCredentials = new SigningCredentials(securityKey, algorithm);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Issuer = _jwtSettings.Issuer,
            IssuedAt = currentTime.DateTime.ToUniversalTime(),
            Audience = _jwtSettings.Audience,
            Expires = (currentTime + _jwtSettings.AccessTokenLifetime).DateTime.ToUniversalTime(),
            SigningCredentials = signingCredentials
        };

        return new JwtSecurityTokenHandler().CreateJwtSecurityToken(tokenDescriptor);
    }
}