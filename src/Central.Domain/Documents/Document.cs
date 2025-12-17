using Central.Domain.Users;

namespace Central.Domain.Documents;

/// <summary>
/// Represents a document with metadata and associated file attachments.
/// </summary>
public sealed record Document
{
    /// <summary>
    /// Gets the unique identifier for the document.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Gets the title of the document.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets the date associated with the document content (not the creation date).
    /// </summary>
    public DateTimeOffset? DocumentDate { get; init; }

    /// <summary>
    /// Gets the textual content or description of the document.
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Gets the original uploaded file.
    /// </summary>
    public DocumentFile? OriginalFile { get; init; }

    /// <summary>
    /// Gets the archived version of the document.
    /// </summary>
    public DocumentFile? ArchiveFile { get; init; }

    /// <summary>
    /// Gets the thumbnail image for the document.
    /// </summary>
    public DocumentFile? Thumbnail { get; init; }

    /// <summary>
    /// Gets the timestamp when the document was added.
    /// </summary>
    public required DateTimeOffset Added { get; init; }

    /// <summary>
    /// Gets the timestamp when the document was last updated.
    /// </summary>
    public required DateTimeOffset Updated { get; init; }

    /// <summary>
    /// Gets the ID of the user who added the document, or null if the user was deleted.
    /// </summary>
    public long? AddedBy { get; init; }

    /// <summary>
    /// Gets the ID of the user who last updated the document, or null if the user was deleted.
    /// </summary>
    public long? UpdatedBy { get; init; }

    /// <summary>
    /// Gets the ID of the document type, or null if no type is assigned.
    /// </summary>
    public long? DocumentTypeId { get; init; }

    /// <summary>
    /// Gets the ID of the correspondent, or null if no correspondent is assigned.
    /// </summary>
    public long? CorrespondentId { get; init; }

    /// <summary>
    /// Gets the collection of tag IDs associated with this document.
    /// </summary>
    public IReadOnlyCollection<long> TagIds { get; init; } = Array.Empty<long>();
}