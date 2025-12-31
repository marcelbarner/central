namespace Central.Domain.Documents.Services;

/// <summary>
/// Domain service for managing document operations.
/// </summary>
public interface IDocumentService
{
    /// <summary>
    /// Creates a new document from an uploaded file.
    /// Automatically generates archive file and thumbnail.
    /// Uses the current authenticated user as creator.
    /// </summary>
    Task<Document> CreateFromFileAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new document with full metadata.
    /// Automatically generates archive file and thumbnail from the original file.
    /// Uses the current authenticated user as creator.
    /// </summary>
    Task<Document> CreateAsync(
        string title,
        DateTimeOffset? documentDate,
        string? content,
        Stream originalFileStream,
        string originalFileName,
        long? documentTypeId,
        long? correspondentId,
        IReadOnlyCollection<long> tagIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing document metadata.
    /// Uses the current authenticated user as updater.
    /// File updates are not supported - files are immutable after document creation.
    /// </summary>
    Task<Document> UpdateAsync(
        long id,
        string title,
        DateTimeOffset? documentDate,
        string? content,
        long? documentTypeId,
        long? correspondentId,
        long? contractId,
        DocumentState state,
        IReadOnlyCollection<long> tagIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a document by its identifier.
    /// </summary>
    Task<Document?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all documents.
    /// </summary>
    Task<IReadOnlyCollection<Document>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a document and all associated files.
    /// </summary>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific file stream for a document.
    /// </summary>
    Task<Stream> GetFileAsync(long id, string fileType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a contract to a document, optionally syncing the correspondent.
    /// </summary>
    /// <param name="documentId">The document ID.</param>
    /// <param name="contractId">The contract ID to assign.</param>
    /// <param name="syncCorrespondent">Whether to update the document's correspondent to match the contract's correspondent.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated document.</returns>
    Task<Document> AssignContractAsync(
        long documentId,
        long contractId,
        bool syncCorrespondent,
        CancellationToken cancellationToken = default);
}