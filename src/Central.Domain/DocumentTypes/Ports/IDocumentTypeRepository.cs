namespace Central.Domain.DocumentTypes.Ports;

/// <summary>
/// Port for managing document type persistence.
/// </summary>
public interface IDocumentTypeRepository
{
    /// <summary>
    /// Adds a new document type to the repository.
    /// </summary>
    /// <param name="documentType">The document type to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added document type with generated ID.</returns>
    Task<DocumentType> AddAsync(DocumentType documentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing document type.
    /// </summary>
    /// <param name="documentType">The document type with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated document type.</returns>
    Task<DocumentType> UpdateAsync(DocumentType documentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a document type by its identifier.
    /// </summary>
    /// <param name="id">The document type identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The document type if found; otherwise null.</returns>
    Task<DocumentType?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a document type by its name.
    /// </summary>
    /// <param name="name">The document type name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The document type if found; otherwise null.</returns>
    Task<DocumentType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all document types.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all document types.</returns>
    Task<IReadOnlyCollection<DocumentType>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a document type by its identifier.
    /// </summary>
    /// <param name="id">The document type identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a document type with the given name exists.
    /// </summary>
    /// <param name="name">The document type name to check.</param>
    /// <param name="excludeId">Optional document type ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a document type with the name exists; otherwise false.</returns>
    Task<bool> ExistsAsync(string name, long? excludeId = null, CancellationToken cancellationToken = default);
}
