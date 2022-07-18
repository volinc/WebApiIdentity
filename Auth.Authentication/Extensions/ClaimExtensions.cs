using System.Security.Claims;
using Auth.Authentication.Constants;

namespace Auth.Authentication.Extensions;

public static class ClaimExtensions
{
    public static string GetSignId(this IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims, nameof(claims));
        return claims.First(x => x.Type == CustomClaimTypes.SignId).Value;
    }

    public static string GetAudience(this IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims, nameof(claims));
        return claims.First(x => x.Type == JwtClaimTypes.Audience).Value;
    }

    public static long GetUserLongId(this IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims, nameof(claims));
        var stringValue = claims.First(x => x.Type == JwtClaimTypes.Subject).Value;
        return long.Parse(stringValue);
    }

    public static Guid GetUserGuidId(this IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims, nameof(claims));
        var stringValue = claims.First(x => x.Type == CustomClaimTypes.UserUuid).Value;
        return Guid.Parse(stringValue);
    }

    public static string GetRole(this IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims, nameof(claims));
        return claims.First(x => x.Type == CustomClaimTypes.UserRole).Value;
    }

    public static long GetCompanyLongId(this IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims, nameof(claims));
        var stringValue = claims.First(x => x.Type == CustomClaimTypes.CompanyId).Value;
        return long.Parse(stringValue);
    }

    public static Guid GetCompanyGuidId(this IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims, nameof(claims));
        var stringValue = claims.First(x => x.Type == CustomClaimTypes.CompanyUuid).Value;
        return Guid.Parse(stringValue);
    }
}