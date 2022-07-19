using System.IdentityModel.Tokens.Jwt;
using Auth.Authentication.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace Auth.Authentication.SessionStorage;

public class DefaultSessionStorage : ISessionStorage
{
    private static readonly Dictionary<string, ClientInfo> EmptyClients = new();

    private readonly Utf8JsonBinarySerializer _binarySerializer;
    private readonly IDistributedCache _cache;
    private readonly CurrentTimeFunc _now;

    public DefaultSessionStorage(
        IDistributedCache cache,
        Utf8JsonBinarySerializer binarySerializer,
        CurrentTimeFunc now)
    {
        _cache = cache;
        _binarySerializer = binarySerializer;
        _now = now;
    }

    public virtual async Task ReplaceAllAsync(
        JwtSecurityToken accessToken,
        JwtSecurityToken refreshToken,
        TimeSpan refreshTokenLifetime,
        ClientInfo clientInfo)
    {
        var userSignIn = new UserSignIn();
        var signId = refreshToken.Claims.GetSignId();
        var absoluteExpiration = _now() + refreshTokenLifetime;
        var userId = accessToken.Claims.GetUserId();

        userSignIn.Items.Add(signId, new SecurityTokens
        {
            AccessJwtSecurityTokenId = accessToken.Id,
            RefreshJwtSecurityTokenId = refreshToken.Id,
            RefreshTokenExpiredAt = absoluteExpiration,
            ClientInfo = clientInfo
        });

        await SaveUserSignInByUserId(userId, userSignIn, absoluteExpiration);
    }

    public virtual async Task AddOrUpdateAsync(
        JwtSecurityToken accessToken,
        JwtSecurityToken refreshToken,
        TimeSpan refreshTokenLifetime,
        ClientInfo clientInfo)
    {
        var userId = refreshToken.Claims.GetUserId();
        var currentTime = _now();
        var absoluteExpiration = currentTime + refreshTokenLifetime;
        var signId = refreshToken.Claims.GetSignId();
        var userSignIn = await GetUserSignInByUserId(userId);

        if (userSignIn is null)
        {
            userSignIn = new UserSignIn();
            userSignIn.Items.Add(signId, new SecurityTokens
            {
                AccessJwtSecurityTokenId = accessToken.Id,
                RefreshJwtSecurityTokenId = refreshToken.Id,
                RefreshTokenExpiredAt = absoluteExpiration,
                ClientInfo = clientInfo
            });
        }
        else
        {
            RemoveExpiredTokens(userSignIn, currentTime);
            RemoveOverflowTokens(userSignIn);

            if (userSignIn.Items.TryGetValue(signId, out var tokens))
            {
                tokens.AccessJwtSecurityTokenId = accessToken.Id;
                tokens.RefreshJwtSecurityTokenId = refreshToken.Id;
                tokens.RefreshTokenExpiredAt = absoluteExpiration;
                tokens.ClientInfo = clientInfo;
            }
            else
            {
                userSignIn.Items.Add(signId, new SecurityTokens
                {
                    AccessJwtSecurityTokenId = accessToken.Id,
                    RefreshJwtSecurityTokenId = refreshToken.Id,
                    RefreshTokenExpiredAt = absoluteExpiration,
                    ClientInfo = clientInfo
                });
            }
        }

        await SaveUserSignInByUserId(userId, userSignIn, absoluteExpiration);
    }

    public virtual async Task<bool> IsRefreshStoredAsync(long userId, string signId, string refreshTokenId)
    {
        var userSignIn = await GetUserSignInByUserId(userId);
        if (userSignIn is null)
            return false;

        if (!userSignIn.Items.TryGetValue(signId, out var tokens))
            return false;

        return tokens.RefreshJwtSecurityTokenId == refreshTokenId;
    }

    public virtual async Task RemoveAsync(long userId, string signId)
    {
        var userSignIn = await GetUserSignInByUserId(userId);
        if (userSignIn is null)
            return;

        userSignIn.Items.Remove(signId);
        if (userSignIn.Items.Count <= 0)
        {
            await RemoveAllTokensByUserId(userId);
            return;
        }

        var absoluteExpiration = userSignIn.Items.Values.Max(x => x.RefreshTokenExpiredAt);

        await SaveUserSignInByUserId(userId, userSignIn, absoluteExpiration);
    }

    public virtual async Task RemoveAllAsync(long userId, string? exceptSignId = null)
    {
        if (exceptSignId is null)
        {
            await RemoveAllTokensByUserId(userId);
        }
        else
        {
            var userSignIn = await GetUserSignInByUserId(userId);
            if (userSignIn is null)
                return;

            var exceptTokens = userSignIn.Items[exceptSignId];
            userSignIn.Items.Clear();
            userSignIn.Items.Add(exceptSignId, exceptTokens);

            await SaveUserSignInByUserId(userId, userSignIn, exceptTokens.RefreshTokenExpiredAt);
        }
    }

    public async Task<Dictionary<string, ClientInfo>> GetAllSessionsAsync(long userId)
    {
        var userSignIn = await GetUserSignInByUserId(userId);
        if (userSignIn is null)
            return EmptyClients;

        return userSignIn
            .Items
            .ToDictionary(x => x.Key, x => x.Value.ClientInfo);
    }

    public async Task<Dictionary<string, ClientInfo>> GetActiveSessionsAsync(long userId, DateTimeOffset currentTime)
    {
        var userSignIn = await GetUserSignInByUserId(userId);
        if (userSignIn is null)
            return EmptyClients;

        return userSignIn
            .Items
            //.Where(p => p.Value.RefreshTokenExpiredAt < currentTime)
            .ToDictionary(x => x.Key, x => x.Value.ClientInfo);
    }

    protected virtual void RemoveOverflowTokens(UserSignIn userSignIn)
    {
        const int overflowCount = 10;
        if (userSignIn.Items.Count < overflowCount)
            return;

        var countToRemove = userSignIn.Items.Count - overflowCount;
        var itemsToRemove =
            userSignIn.Items.OrderBy(x => x.Value.RefreshTokenExpiredAt).Take(countToRemove).ToArray();

        foreach (var (key, _) in itemsToRemove)
            userSignIn.Items.Remove(key);
    }

    protected virtual void RemoveExpiredTokens(UserSignIn userSignIn, DateTimeOffset currentTime)
    {
        var itemsToRemove =
            userSignIn.Items.Where(x => x.Value.RefreshTokenExpiredAt < currentTime).ToArray();

        foreach (var (key, _) in itemsToRemove)
            userSignIn.Items.Remove(key);
    }

    private async Task RemoveAllTokensByUserId(long userId)
    {
        var key = SessionStorageHelper.Key(userId);

        await _cache.RemoveAsync(key);
    }

    private async Task<UserSignIn?> GetUserSignInByUserId(long userId)
    {
        var key = SessionStorageHelper.Key(userId);
        var bytes = await _cache.GetAsync(key);

        return bytes == null
            ? null
            : _binarySerializer.Deserialize<UserSignIn>(bytes);
    }

    private async Task SaveUserSignInByUserId(long userId, UserSignIn userSignIn, DateTimeOffset absoluteExpiration)
    {
        var key = SessionStorageHelper.Key(userId);
        var bytes = _binarySerializer.Serialize(userSignIn);

        await _cache.SetAsync(key, bytes, new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration
        });
    }
}