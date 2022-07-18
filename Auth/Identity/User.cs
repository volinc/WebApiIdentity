using Microsoft.AspNetCore.Identity;

namespace Auth.Identity;

public class User : IdentityUser<long>
{
    public string? FirstName { get; set; }
    public string? GivenName { get; set; }
    public string? Biography { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}