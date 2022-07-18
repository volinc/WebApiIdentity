namespace Auth.Authentication.Constants;

public static class CustomClaimTypes
{
    public const string UserRole = "role";
    public const string SignId = "http://company-name/schema/claims/sign-id";
    public const string TokenType = "http://company-name/schema/claims/token-type";
    public const string UserUuid = "http://company-name/schema/claims/user-uuid";
    public const string CompanyId = "http://company-name/schema/claims/organization-id";
    public const string CompanyUuid = "http://company-name/schema/claims/organization-uuid";
}