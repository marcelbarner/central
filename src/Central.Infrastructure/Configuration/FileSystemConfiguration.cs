namespace Central.Infrastructure.Configuration;

/// <summary>
/// Configuration for file system storage.
/// </summary>
public sealed record FileSystemConfiguration
{
    /// <summary>
    /// Gets the base path for media storage.
    /// </summary>
    public required string Media { get; init; }
}
