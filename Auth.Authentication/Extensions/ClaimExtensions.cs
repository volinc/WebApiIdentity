using System.Security.Claims;
using Auth.Authentication.Constants;

namespace Auth.Authentication.Extensions;

public static class ClaimExtensions
{
    public static long GetUserId(this IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims, nameof(claims));
        var stringValue = claims.First(x => x.Type == CustomClaimTypes.NameId).Value;
        return long.Parse(stringValue);
    }

    public static string[] GetRoles(this IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims, nameof(claims));
        return claims.Where(x => x.Type == CustomClaimTypes.Role).Select(x => x.Value).ToArray();
    }

    public static string GetSignId(this IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims, nameof(claims));
        return claims.First(x => x.Type == CustomClaimTypes.SignId).Value;
    }

    public static string GetAudience(this IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims, nameof(claims));
        return claims.First(x => x.Type == CustomClaimTypes.Audience).Value;
    }
    
    public static Guid GetCompanyId(this IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims, nameof(claims));
        var stringValue = claims.First(x => x.Type == CustomClaimTypes.CompanyId).Value;
        return Guid.Parse(stringValue);
    }
}