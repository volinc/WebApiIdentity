namespace Auth.Data;

public static class Seed
{
    public static readonly DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;
    public const long AdminUserId = 1;
    public const long AdminRoleId = 1;
    public const long CustomerRoleId = 2;
}