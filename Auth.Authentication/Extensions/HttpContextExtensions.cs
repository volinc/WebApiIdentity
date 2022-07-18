using System.Security.Authentication;
using Auth.Authentication.Constants;
using Auth.Authentication.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Auth.Authentication.Extensions;

public static class HttpContextExtensions
{
    public static string? GetAccessToken(this HttpContext? httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext, nameof(httpContext));
        
        if (!httpContext.Request.Headers.TryGetValue(HeaderNames.Authorization, out var value))
            throw new AuthenticationException(Messages.MissingAuthorizationHeader);

        if (!AccessTokenHelper.TryGetToken(value, JwtBearerDefaults.AuthenticationScheme, out var token))
            throw new AuthenticationException(Messages.InvalidAuthenticationScheme);

        return token;
    }

    public static string? GetUserAgent(this HttpContext? httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext, nameof(httpContext));
        
        var userAgent = string.Join(",", httpContext.Request.Headers.UserAgent);

        return string.IsNullOrWhiteSpace(userAgent) ? null : userAgent;
    }

    public static string? GetClientIp(this HttpContext? httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext, nameof(httpContext));

        if (httpContext.Request.Headers.TryGetValue("cf-connecting-ip", out var value))
            return value;

        if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out value))
            return value;

        return null;
    }
}