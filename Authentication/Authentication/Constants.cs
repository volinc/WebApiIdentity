namespace Authentication;

public static class Constants
{
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
        public const string EmailVerified = "email_verified";
    }

    public static class CustomClaimTypes
    {
        public const string UserRole = "role";
        public const string SignId = "http://pocketvc.online/schema/claims/sign-id";
        public const string TokenType = "http://pocketvc.online/schema/claims/token-type";
        public const string UserUuid = "http://pocketvc.online/schema/claims/user-uuid";
        public const string CompanyId = "http://pocketvc.online/schema/claims/organization-id";
        public const string CompanyUuid = "http://pocketvc.online/schema/claims/organization-uuid";
    }

    public static class JwtTokenTypes
    {
        public const string Access = "access";
        public const string Refresh = "refresh";
    }

    public static class Messages
    {
        public const string MissingAuthorizationHeader = "Missing authorization header";
        public const string InvalidAuthenticationScheme = "Invalid authentication scheme";
        public const string FailToResolvePrincipal = "Unable to resolve principal";
        public const string IncorrectGrantType = "Incorrect grant type";
        public const string IncorrectAccessToken = "Incorrect access token";
        public const string IncorrectRefreshToken = "Incorrect refresh token";
    }
}