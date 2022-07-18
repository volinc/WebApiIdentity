using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Auth.Authentication.Constants;

public static class CustomClaimTypes
{
    public const string ClaimNamespace = "http://company-name/schema/claims";
    public const string SignId = $"{ClaimNamespace}/sign-id";
    public const string TokenType = $"{ClaimNamespace}/token-type";
    public const string CompanyId = $"{ClaimNamespace}/company-id";
    
    #region // https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

    public const string Role = ClaimTypes.Role;
    public const string TokenId = JwtRegisteredClaimNames.Jti;
    public const string NameId = JwtRegisteredClaimNames.NameId;
    public const string Name = JwtRegisteredClaimNames.Name;
    public const string Issuer = JwtRegisteredClaimNames.Iss;
    public const string IssuedAt = JwtRegisteredClaimNames.Iat;
    public const string Audience = JwtRegisteredClaimNames.Aud;
    public const string Email = JwtRegisteredClaimNames.Email;
    public const string EmailVerified = "email_verified";

    #endregion
}