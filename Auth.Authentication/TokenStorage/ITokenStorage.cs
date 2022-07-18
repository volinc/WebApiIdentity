using System.IdentityModel.Tokens.Jwt;

namespace Auth.Authentication.TokenStorage;

public interface ITokenStorage
{
    Task ReplaceAllAsync(
        JwtSecurityToken accessToken,
        JwtSecurityToken refreshToken,
        TimeSpan refreshTokenLifetime,
        Client client);

    Task AddOrUpdateAsync(
        JwtSecurityToken accessToken,
        JwtSecurityToken refreshToken,
        TimeSpan tokenLifetime,
        Client client);

    Task<bool> IsRefreshStoredAsync(long userId, string signId, string refreshTokenId);

    Task RemoveAsync(long userId, string signId);
    Task RemoveAllAsync(long userId, string? exceptSignId = null);

    Task<Dictionary<string, Client>> GetAllSessionsAsync(long userId);
    Task<Dictionary<string, Client>> GetActiveSessionsAsync(long userId, DateTimeOffset currentTime);
}