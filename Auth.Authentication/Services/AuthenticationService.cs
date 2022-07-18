using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Text;
using Auth.Authentication.Constants;
using Auth.Authentication.Extensions;
using Auth.Authentication.Handlers;
using Auth.Authentication.TokenStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Authentication.Services;

public class AuthenticationService
{
    public delegate Task<IdentityUser<long>> ValidateAsync(string grantType, long userId);

    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly JwtSettings _jwtSettings;
    private readonly AccessTokenHandler _accessTokenHandler;
    private readonly RefreshTokenHandler _refreshTokenHandler;
    private readonly ITokenStorage _tokenStorage;

    public AuthenticationService(IHttpContextAccessor httpContextAccessor,
        ITokenStorage tokenStorage,
        JwtSettings jwtSettings,
        AccessTokenHandler accessTokenHandler,
        RefreshTokenHandler refreshTokenHandler)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtSettings = jwtSettings;
        _accessTokenHandler = accessTokenHandler;
        _refreshTokenHandler = refreshTokenHandler;
        _tokenStorage = tokenStorage;
    }

    public async Task<AccessTokenResponse> CreateTokensAsync(AccessTokenRequest request, ValidateAsync validate, string userAgent, string ip, string geolocation)
    {
        var (user, signId) = request.GrantType switch
        {
            GrantTypes.Password => await AuthenticateByPasswordAsync(validate),
            GrantTypes.RefreshToken => await AuthenticateByRefreshTokenAsync(request.RefreshToken, validate),
            _ => throw new AuthenticationException(Messages.IncorrectGrantType)
        };

        var accessToken = _accessTokenHandler.CreateToken(signId, user);
        var refreshToken = _refreshTokenHandler.CreateToken(signId, user);

        await _tokenStorage.AddOrUpdateAsync(accessToken, refreshToken, _jwtSettings.RefreshTokenLifetime,
            new Client
            {
                Name = userAgent,
                Ip = ip,
                Geolocation = geolocation
            });

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        return new AccessTokenResponse
        {
            AccessToken = jwtSecurityTokenHandler.WriteToken(accessToken),
            RefreshToken = jwtSecurityTokenHandler.WriteToken(refreshToken),
            ExpiresIn = (int) _jwtSettings.AccessTokenLifetime.TotalSeconds,
            TokenType = TokenTypes.Bearer
        };
    }
    
    private static async Task<(IdentityUser<long> user, string signId)> AuthenticateByPasswordAsync(ValidateAsync validate)
    {
        var user = await validate(GrantTypes.Password, default);
        var signId = Guid.NewGuid().ToString("N");
        return (user, signId);
    }

    private async Task<(IdentityUser<long> user, string signId)> AuthenticateByRefreshTokenAsync(string? refreshToken, ValidateAsync validate)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        jwtSecurityTokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.TokenPassword)),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = _jwtSettings.ClockSkew

        }, out var validatedToken);
        
        var securityToken = (JwtSecurityToken)validatedToken;

        var userId = securityToken.Claims.GetUserLongId();
        var signId = securityToken.Claims.GetSignId();
        
        if (!await _tokenStorage.IsRefreshStoredAsync(userId, signId, securityToken.Id))
            throw new AuthenticationException(Messages.IncorrectRefreshToken);

        var user = await validate(GrantTypes.RefreshToken, userId);
        return (user, signId);
    }

    private JwtSecurityToken GetAccessToken()
    {
        var token = _httpContextAccessor.HttpContext?.GetAccessToken();
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        return jwtSecurityTokenHandler.ReadJwtToken(token);
    }

    public Task<Dictionary<string, Client>> GetAllSessionsAsync(long userId) => _tokenStorage.GetAllSessionsAsync(userId);

    public Task<Dictionary<string, Client>> GetActiveSessionsAsync(long userId) => _tokenStorage.GetActiveSessionsAsync(userId, DateTimeOffset.Now);

    public Task RemoveAllTokensAsync(long userId, string exceptSignId) => _tokenStorage.RemoveAllAsync(userId, exceptSignId);

    public Task RemoveTokensAsync(long userId, string? signIdForDelete)
    {
        var signId = signIdForDelete ?? GetAccessToken().Claims.GetSignId();

        return _tokenStorage.RemoveAsync(userId, signId);
    }
}