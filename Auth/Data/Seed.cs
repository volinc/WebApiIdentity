namespace Auth.Data;

public static class Seed
{
    public static readonly DateTimeOffset CreatedAt = new(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
    public const long AdminUserId = 1;
    public const long AdminRoleId = 1;
    public const long CustomerRoleId = 2;
}