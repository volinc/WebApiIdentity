namespace WebApiIdentity.Data;

public static class Seed
{
    public static readonly DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;
    public static readonly Guid SeedUserId = Guid.Parse("{AB9A54CA-621E-4589-BD43-6D8108D3B00E}");
    public static readonly Guid AdminRoleId = Guid.Parse("{38B8693A-0C0E-4D46-9A71-FC67CD1106AE}");
}