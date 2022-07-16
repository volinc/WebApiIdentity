namespace Authentication;

public class JwtSettings
{
    public string AccessTokenSecret { get; set; } = null!;
    public string RefreshTokenSecret { get; set; } = null!;
    public TimeSpan AccessTokenLifetime { get; set; }
    public TimeSpan RefreshTokenLifetime { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public TimeSpan ClockSkew { get; set; }
}