using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;

namespace WebApiIdentity.Domain.Identity;

public class UserToken : IdentityUserToken<Guid>
{
    public string RefreshToken { get; set; } = null!;
    public string? UserAgent { get; set; }
    public Point Location { get; set; } = Point.Empty;
}