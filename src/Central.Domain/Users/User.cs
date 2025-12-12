using Microsoft.AspNetCore.Identity;

namespace Central.Domain.Users;

/// <summary>
/// Represents a user in the system.
/// Uses ASP.NET Core Identity with long as the primary key type.
/// </summary>
public sealed class User : IdentityUser<long>
{
    /// <summary>
    /// Gets or sets the user's display name.
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets when the user was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets when the user was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the user last logged in.
    /// </summary>
    public DateTimeOffset? LastLoginAt { get; set; }
}
