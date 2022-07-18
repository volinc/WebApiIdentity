using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Authentication.Constants;
using Auth.Authentication.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Authentication.Handlers;

public class RefreshTokenHandler
{
    private readonly JwtSettings _jwtSettings;
    private readonly CurrentTimeFunc _now;

    public RefreshTokenHandler(JwtSettings jwtSettings, CurrentTimeFunc now)
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
            new Claim(CustomClaimTypes.TokenType, JwtTokenTypes.Refresh)
        });

        var securityKey = SecurityKeyHelper.Create(_jwtSettings.TokenPassword);
        const string algorithm = SecurityAlgorithms.HmacSha256Signature;
        var signingCredentials = new SigningCredentials(securityKey, algorithm);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Expires = (_now() + _jwtSettings.RefreshTokenLifetime).DateTime.ToUniversalTime(),
            SigningCredentials = signingCredentials
        };

        return new JwtSecurityTokenHandler().CreateJwtSecurityToken(tokenDescriptor);
    }
}