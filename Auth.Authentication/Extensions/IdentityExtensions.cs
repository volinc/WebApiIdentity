using System.Security.Claims;
using System.Security.Principal;
using Auth.Authentication.Constants;

namespace Auth.Authentication.Extensions;

public static class IdentityExtensions
{
    public static long GetUserId(this IIdentity identity) => identity.GetClaims().GetUserId();

    public static string[] GetRoles(this IIdentity identity) => identity.GetClaims().GetRoles();

    public static string GetSignId(this IIdentity identity) => identity.GetClaims().GetSignId();

    public static string GetAudience(this IIdentity identity) => identity.GetClaims().GetAudience();
    
    public static Guid GetCompanyId(this IIdentity identity) => identity.GetClaims().GetCompanyId();

    public static bool TryGetCompanyId(this IIdentity identity, out Guid companyId)
    {
        var claim = identity.GetClaims().FirstOrDefault(x => x.Type == CustomClaimTypes.CompanyId);

        if (claim != null && Guid.TryParse(claim.Value, out companyId))
            return true;

        companyId = default;
        return false;
    }

    private static IEnumerable<Claim> GetClaims(this IIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(identity, nameof(identity));
        return ((ClaimsIdentity) identity).Claims;
    }
}