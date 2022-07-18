namespace Auth.Authentication.TokenStorage;

public class SecurityTokens
{
    public Client Client { get; set; } = null!;
    public string AccessJwtSecurityTokenId { get; set; } = null!;
    public string RefreshJwtSecurityTokenId { get; set; } = null!;
    public DateTimeOffset RefreshTokenExpiredAt { get; set; }
}