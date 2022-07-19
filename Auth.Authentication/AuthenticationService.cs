using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using Auth.Authentication.Constants;
using Auth.Authentication.Extensions;
using Auth.Authentication.Handlers;
using Auth.Authentication.Helpers;
using Auth.Authentication.SessionStorage;
using Auth.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Authentication;

public class AuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISessionStorage _sessionStorage;
    private readonly JwtSettings _jwtSettings;
    private readonly HttpHeaderSettings _httpHeaderSettings;
    private readonly AccessTokenHandler _accessTokenHandler;
    private readonly RefreshTokenHandler _refreshTokenHandler;
    private readonly CurrentTimeFunc _now;

    public AuthenticationService(UserManager<User> userManager, 
        IHttpContextAccessor httpContextAccessor,
        ISessionStorage sessionStorage,
        JwtSettings jwtSettings,
        HttpHeaderSettings httpHeaderSettings,
        AccessTokenHandler accessTokenHandler,
        RefreshTokenHandler refreshTokenHandler,
        CurrentTimeFunc now)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _sessionStorage = sessionStorage;
        _jwtSettings = jwtSettings;
        _httpHeaderSettings = httpHeaderSettings;
        _accessTokenHandler = accessTokenHandler;
        _refreshTokenHandler = refreshTokenHandler;
        _now = now;
    }

    public async Task<AccessTokenResponse> AuthenticateAsync(AccessTokenRequest request)
    {
        var (user, signId) = request.GrantType switch
        {
            GrantTypes.Password => await AuthenticateByPasswordAsync(request.Username, request.Password),
            GrantTypes.RefreshToken => await AuthenticateByRefreshTokenAsync(request.RefreshToken),
            _ => throw new AuthenticationException(Messages.IncorrectGrantType)
        };

        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _accessTokenHandler.CreateToken(user, roles.ToArray(), signId);
        var refreshToken = _refreshTokenHandler.CreateToken(user, signId);

        var client = GetClientInfo();
        await _sessionStorage.AddOrUpdateAsync(accessToken, refreshToken, _jwtSettings.RefreshTokenLifetime, client);

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        return new AccessTokenResponse
        {
            AccessToken = jwtSecurityTokenHandler.WriteToken(accessToken),
            RefreshToken = jwtSecurityTokenHandler.WriteToken(refreshToken),
            ExpiresIn = (int)_jwtSettings.AccessTokenLifetime.TotalSeconds,
            TokenType = "bearer"
        };
    }

    private ClientInfo GetClientInfo()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userAgent = httpContext.GetUserAgent();
        var ip = httpContext.GetClientIp(_httpHeaderSettings.IPHeaderName);
        var ipCountry = httpContext.GetClientIpCountry(_httpHeaderSettings.IPCountryHeaderName);
        var client = new ClientInfo
        {
            UserAgent = userAgent,
            Ip = ip,
            IpCountry = ipCountry
        };
        return client;
    }

    private async Task<(User user, string sessionId)> AuthenticateByPasswordAsync(string? userName, string? password)
    {
        var user = await _userManager.FindByEmailAsync(userName);
        if (user == null)
            throw new InvalidOperationException("Invalid username or password");

        var validPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!validPassword)
            throw new InvalidOperationException("Invalid username or password");

        var signId = Guid.NewGuid().ToString("N");
        return (user, signId);
    }

    private async Task<(User user, string sessionId)> AuthenticateByRefreshTokenAsync(string? refreshToken)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        jwtSecurityTokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SecurityKeyHelper.Create(_jwtSettings.TokenPassword),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = _jwtSettings.ClockSkew

        }, out var validatedToken);

        var securityToken = (JwtSecurityToken)validatedToken;

        var userId = securityToken.Claims.GetUserId();
        var signId = securityToken.Claims.GetSignId();

        if (!await _sessionStorage.IsRefreshStoredAsync(userId, signId, securityToken.Id))
            throw new AuthenticationException(Messages.IncorrectRefreshToken);

        var user = await _userManager.FindByIdAsync(userId.ToString());
        return (user, signId);
    }

    public Task<Dictionary<string, ClientInfo>> GetAllSessionsAsync(long userId) => _sessionStorage.GetAllSessionsAsync(userId);

    public Task<Dictionary<string, ClientInfo>> GetActiveSessionsAsync(long userId)
    {
        return _sessionStorage.GetActiveSessionsAsync(userId, _now());
    }

    public Task RemoveAllSessionsAsync(long userId, string exceptSessionId)
    {
        return _sessionStorage.RemoveAllAsync(userId, exceptSessionId);
    }

    public Task RemoveSessionAsync(long userId, string? sessionIdToRemove)
    {
        string? signId;

        if (sessionIdToRemove != null)
        {
            signId = sessionIdToRemove;
        }
        else
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var token = httpContext.GetAccessToken();
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            signId = jwtSecurityTokenHandler.ReadJwtToken(token).Claims.GetSignId();
        }

        return _sessionStorage.RemoveAsync(userId, signId);
    }
}