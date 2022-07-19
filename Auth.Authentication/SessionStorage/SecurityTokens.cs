namespace Auth.Authentication.SessionStorage;

public class SecurityTokens
{
    public ClientInfo ClientInfo { get; set; } = null!;
    public string AccessJwtSecurityTokenId { get; set; } = null!;
    public string RefreshJwtSecurityTokenId { get; set; } = null!;
    public DateTimeOffset RefreshTokenExpiredAt { get; set; }
}