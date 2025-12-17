namespace Central.Domain.DocumentTypes;

/// <summary>
/// Represents a document type that can be assigned to documents for classification.
/// </summary>
public sealed record DocumentType
{
    /// <summary>
    /// Gets the unique identifier for the document type.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Gets the name of the document type.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the optional description of the document type.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the timestamp when the document type was created.
    /// </summary>
    public required DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the timestamp when the document type was last updated.
    /// </summary>
    public required DateTimeOffset Updated { get; init; }
}