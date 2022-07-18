namespace Auth.Authentication.Constants;

// https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
public static class JwtClaimTypes
{
    public const string TokenId = "jti";
    public const string Subject = "sub";
    public const string Name = "name";
    public const string Audience = "aud";
    public const string Issuer = "iss";
    public const string IssuedAt = "iat";
    public const string Email = "email";
}