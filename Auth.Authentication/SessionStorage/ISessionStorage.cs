using System.IdentityModel.Tokens.Jwt;

namespace Auth.Authentication.SessionStorage;

public interface ISessionStorage
{
    Task ReplaceAllAsync(
        JwtSecurityToken accessToken,
        JwtSecurityToken refreshToken,
        TimeSpan refreshTokenLifetime,
        ClientInfo clientInfo);

    Task AddOrUpdateAsync(
        JwtSecurityToken accessToken,
        JwtSecurityToken refreshToken,
        TimeSpan tokenLifetime,
        ClientInfo clientInfo);

    Task<bool> IsRefreshStoredAsync(long userId, string signId, string refreshTokenId);

    Task RemoveAsync(long userId, string signId);
    Task RemoveAllAsync(long userId, string? exceptSignId = null);

    Task<Dictionary<string, ClientInfo>> GetAllSessionsAsync(long userId);
    Task<Dictionary<string, ClientInfo>> GetActiveSessionsAsync(long userId, DateTimeOffset currentTime);
}