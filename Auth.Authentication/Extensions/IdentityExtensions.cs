using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using Auth.Authentication.Constants;

namespace Auth.Authentication.Extensions;

public static class IdentityExtensions
{
    public static string GetSignId(this IIdentity identity) => identity.GetClaims().GetSignId();

    public static string GetAudience(this IIdentity identity) => identity.GetClaims().GetAudience();

    public static long GetId(this IIdentity identity) => identity.GetClaims().GetUserLongId();

    public static string GetRole(this IIdentity identity) => identity.GetClaims().GetRole();
    
    public static long GetCompanyId(this IIdentity identity) => identity.GetClaims().GetCompanyLongId();

    public static Guid GetCompanyUuid(this IIdentity identity) => identity.GetClaims().GetCompanyGuidId();

    public static bool TryGetCompanyId(this IIdentity identity, out long companyId)
    {
        var claim = identity.GetClaims().FirstOrDefault(x => x.Type == CustomClaimTypes.CompanyId);

        if (claim != null &&
            long.TryParse(claim.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out companyId))
            return true;

        companyId = default;
        return false;
    }

    public static bool TryGetCompanyUuid(this IIdentity identity, out Guid companyUuid)
    {
        var claim = identity.GetClaims().FirstOrDefault(x => x.Type == CustomClaimTypes.CompanyUuid);

        if (claim != null && Guid.TryParse(claim.Value, out companyUuid))
            return true;

        companyUuid = default;
        return false;
    }

    private static IEnumerable<Claim> GetClaims(this IIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(identity, nameof(identity));
        return ((ClaimsIdentity) identity).Claims;
    }
}