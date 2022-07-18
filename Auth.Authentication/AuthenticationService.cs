using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using Auth.Authentication.Constants;
using Auth.Authentication.Extensions;
using Auth.Authentication.Handlers;
using Auth.Authentication.Helpers;
using Auth.Authentication.TokenStorage;
using Auth.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Authentication;

public class AuthenticationService
{
    public delegate Task<User> ValidateAsync(string grantType, long userId);

    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenStorage _tokenStorage;
    private readonly JwtSettings _jwtSettings;
    private readonly AccessTokenHandler _accessTokenHandler;
    private readonly RefreshTokenHandler _refreshTokenHandler;
    
    public AuthenticationService(UserManager<User> userManager, 
        IHttpContextAccessor httpContextAccessor,
        ITokenStorage tokenStorage,
        JwtSettings jwtSettings,
        AccessTokenHandler accessTokenHandler,
        RefreshTokenHandler refreshTokenHandler)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _tokenStorage = tokenStorage;
        _jwtSettings = jwtSettings;
        _accessTokenHandler = accessTokenHandler;
        _refreshTokenHandler = refreshTokenHandler;
    }

    public async Task<AccessTokenResponse> CreateTokensAsync(AccessTokenRequest request)
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

        var httpContext = _httpContextAccessor.HttpContext;
        var userAgent = httpContext.GetUserAgent();
        var clientIp = httpContext.GetClientIp();

        await _tokenStorage.AddOrUpdateAsync(accessToken, refreshToken, _jwtSettings.RefreshTokenLifetime,
            new Client
            {
                UserAgent = userAgent,
                Ip = clientIp,
                Location = null
            });

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        return new AccessTokenResponse
        {
            AccessToken = jwtSecurityTokenHandler.WriteToken(accessToken),
            RefreshToken = jwtSecurityTokenHandler.WriteToken(refreshToken),
            ExpiresIn = (int)_jwtSettings.AccessTokenLifetime.TotalSeconds,
            TokenType = "bearer"
        };
    }

    private async Task<(User user, string signId)> AuthenticateByPasswordAsync(string? userName, string? password)
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

    private async Task<(User user, string signId)> AuthenticateByRefreshTokenAsync(string? refreshToken)
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

        if (!await _tokenStorage.IsRefreshStoredAsync(userId, signId, securityToken.Id))
            throw new AuthenticationException(Messages.IncorrectRefreshToken);

        var user = await _userManager.FindByIdAsync(userId.ToString());
        return (user, signId);
    }

    public Task<Dictionary<string, Client>> GetAllSessionsAsync(long userId) => _tokenStorage.GetAllSessionsAsync(userId);

    public Task<Dictionary<string, Client>> GetActiveSessionsAsync(long userId) => _tokenStorage.GetActiveSessionsAsync(userId, DateTimeOffset.Now);

    public Task RemoveAllTokensAsync(long userId, string exceptSignId) => _tokenStorage.RemoveAllAsync(userId, exceptSignId);

    public Task RemoveTokensAsync(long userId, string? signIdForDelete)
    {
        string? signId;

        if (signIdForDelete != null)
        {
            signId = signIdForDelete;
        }
        else
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var token = httpContext.GetAccessToken();
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            signId = jwtSecurityTokenHandler.ReadJwtToken(token).Claims.GetSignId();
        }

        return _tokenStorage.RemoveAsync(userId, signId);
    }
}