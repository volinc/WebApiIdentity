using System.Security.Authentication;
using Auth.Authentication.Constants;
using Auth.Authentication.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace Auth.Authentication.Extensions;

internal static class HttpContextExtensions
{
    public static string? GetAccessToken(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext, nameof(httpContext));
        
        if (!AccessTokenHelper.TryGetAuthorization(httpContext.Request, out var authorization))
            throw new AuthenticationException(Messages.MissingAuthorizationHeader);

        if (!AccessTokenHelper.TryGetToken(authorization, JwtBearerDefaults.AuthenticationScheme, out var token))
            throw new AuthenticationException(Messages.InvalidAuthenticationScheme);

        return token;
    }
}