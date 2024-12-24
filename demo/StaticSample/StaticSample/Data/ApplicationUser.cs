using Microsoft.AspNetCore.Identity;

namespace StaticSample.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public const string DefaultEmail = "demo@static.com";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
