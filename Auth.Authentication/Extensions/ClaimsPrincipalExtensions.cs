using System.Security.Claims;

namespace Auth.Authentication.Extensions;

public static class ClaimsPrincipalExtensions
{
    internal static string GetSignId(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(Constants.CustomClaimTypes.SignId);

    public static string GetAudience(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(Constants.JwtClaimTypes.Audience);

    public static string GetRole(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(Constants.CustomClaimTypes.UserRole);

    public static long GetId(this ClaimsPrincipal principal)
    {
        var stringValue = principal.FindFirstValue(Constants.JwtClaimTypes.Subject);
        return long.Parse(stringValue);
    }

    public static Guid GetUuid(this ClaimsPrincipal principal)
    {
        var stringValue = principal.FindFirstValue(Constants.CustomClaimTypes.UserUuid);
        return Guid.Parse(stringValue);
    }

    public static long GetCompanyId(this ClaimsPrincipal principal)
    {
        var stringValue = principal.FindFirstValue(Constants.CustomClaimTypes.CompanyId);
        return long.Parse(stringValue);
    }

    public static Guid GetCompanyUuid(this ClaimsPrincipal principal)
    {
        var stringValue = principal.FindFirstValue(Constants.CustomClaimTypes.CompanyUuid);
        return Guid.Parse(stringValue);
    }

    public static bool TryGetCompanyId(this ClaimsPrincipal principal, out long companyId)
    {
        var stringValue = principal.FindFirstValue(Constants.CustomClaimTypes.CompanyId);
        return long.TryParse(stringValue, out companyId);
    }

    public static bool TryGetCompanyUuid(this ClaimsPrincipal principal, out Guid companyUuid)
    {
        var stringValue = principal.FindFirstValue(Constants.CustomClaimTypes.CompanyUuid);
        return Guid.TryParse(stringValue, out companyUuid);
    }

    public static bool TryGetPhone(this ClaimsPrincipal principal, out string phone)
    {
        phone = principal.FindFirstValue(Constants.JwtClaimTypes.Email);
        return phone != null;
    }

    public static DateTimeOffset GetIssuedAt(this ClaimsPrincipal principal)
    {
        var stringValue = principal.FindFirstValue(Constants.JwtClaimTypes.IssuedAt);
        return DateTimeOffset.FromUnixTimeSeconds(long.Parse(stringValue));
    }
}