using Microsoft.AspNetCore.Identity;

namespace Auth.Identity;

public class UserToken : IdentityUserToken<long>
{
    //public string RefreshToken { get; set; } = null!;
    //public string? UserAgent { get; set; }
    //public Point Location { get; set; } = Point.Empty;
}