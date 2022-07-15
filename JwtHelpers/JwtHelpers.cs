using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApiIdentity.Models;

namespace WebApiIdentity.JwtHelpers;

public static class JwtHelpers
{
    public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts, Guid id)
    {
        IEnumerable<Claim> claims = new[]
        {
            new("Id", userAccounts.Id.ToString()),
            new Claim(ClaimTypes.Name, userAccounts.UserName),
            new Claim(ClaimTypes.Email, userAccounts.EmailId),
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddDays(1).ToString("MMM ddd dd yyyy HH:mm:ss tt"))
        };
        return claims;
    }

    public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts, out Guid id)
    {
        id = Guid.NewGuid();
        return GetClaims(userAccounts, id);
    }

    public static UserTokens GenTokenKey(UserTokens model, JwtSettings jwtSettings)
    {
        var userToken = new UserTokens();
        if (model == null) 
            throw new ArgumentException(nameof(model));

        // Get secret key
        var key = Encoding.ASCII.GetBytes(jwtSettings.IssuerSigningKey);
        var id = Guid.Empty;
        var expireTime = DateTime.UtcNow.AddDays(1);
        userToken.Validity = expireTime.TimeOfDay;
        var jwToken = new JwtSecurityToken(issuer: jwtSettings.ValidIssuer, audience: jwtSettings.ValidAudience,
            claims: GetClaims(model, out id), notBefore: new DateTimeOffset(DateTime.Now).DateTime,
            expires: new DateTimeOffset(expireTime).DateTime,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
        userToken.Token = new JwtSecurityTokenHandler().WriteToken(jwToken);
        userToken.UserName = model.UserName;
        userToken.Id = model.Id;
        userToken.GuidId = id;
        return userToken;
    }
}