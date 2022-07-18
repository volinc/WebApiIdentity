using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Authentication.Constants;
using Auth.Authentication.Helpers;
using Auth.Identity;
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

    public JwtSecurityToken CreateToken(User user, string signId)
    {
        var tokenId = Guid.NewGuid().ToString("N");
        var userId = user.Id.ToString();

        var subject = new ClaimsIdentity(new[]
        {
            new Claim(CustomClaimTypes.TokenType, TokenTypes.Refresh),
            new Claim(CustomClaimTypes.TokenId, tokenId),
            new Claim(CustomClaimTypes.SignId, signId),
            new Claim(CustomClaimTypes.NameId, userId)
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