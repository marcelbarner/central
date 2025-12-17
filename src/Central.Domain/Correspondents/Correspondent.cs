namespace Central.Domain.Correspondents;

/// <summary>
/// Represents a correspondent (sender or recipient) that can be associated with documents.
/// </summary>
public sealed record Correspondent
{
    /// <summary>
    /// Gets the unique identifier for the correspondent.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Gets the name of the correspondent.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the optional description of the correspondent.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the timestamp when the correspondent was created.
    /// </summary>
    public required DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the timestamp when the correspondent was last updated.
    /// </summary>
    public required DateTimeOffset Updated { get; init; }
}