using System.Net.Http.Headers;
using Microsoft.Extensions.Primitives;

namespace Auth.Authentication.Helpers;

internal static class AccessTokenHelper
{
    public static bool TryGetToken(StringValues authorization, string authenticationScheme, out string? token)
    {
        token = null;

        if (!AuthenticationHeaderValue.TryParse(authorization, out var authentication))
            return false;

        if (!authenticationScheme.Equals(authentication.Scheme, StringComparison.OrdinalIgnoreCase))
            return false;

        token = authentication.Parameter;
        return true;
    }
}