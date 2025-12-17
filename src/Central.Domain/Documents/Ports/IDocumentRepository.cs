namespace Central.Domain.Documents.Ports;

/// <summary>
/// Port for managing document persistence.
/// </summary>
public interface IDocumentRepository
{
    /// <summary>
    /// Adds a new document to the repository.
    /// </summary>
    /// <param name="document">The document to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added document with generated ID.</returns>
    Task<Document> AddAsync(Document document, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing document.
    /// </summary>
    /// <param name="document">The document with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated document.</returns>
    Task<Document> UpdateAsync(Document document, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a document by its identifier.
    /// </summary>
    /// <param name="id">The document identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The document if found; otherwise null.</returns>
    Task<Document?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all documents.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all documents.</returns>
    Task<IReadOnlyCollection<Document>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a document by its identifier.
    /// </summary>
    /// <param name="id">The document identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}