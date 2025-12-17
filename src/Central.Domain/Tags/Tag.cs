namespace Central.Domain.Tags;

/// <summary>
/// Represents a tag that can be applied to documents for categorization and organization.
/// </summary>
public sealed record Tag
{
    /// <summary>
    /// Gets the unique identifier for the tag.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Gets the name of the tag.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the optional description of the tag.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the timestamp when the tag was created.
    /// </summary>
    public required DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the timestamp when the tag was last updated.
    /// </summary>
    public required DateTimeOffset Updated { get; init; }
}