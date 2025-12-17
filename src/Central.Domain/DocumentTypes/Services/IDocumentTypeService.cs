namespace Central.Domain.DocumentTypes.Services;

/// <summary>
/// Domain service for managing document type operations.
/// </summary>
public interface IDocumentTypeService
{
    /// <summary>
    /// Creates a new document type.
    /// </summary>
    /// <param name="name">The document type name.</param>
    /// <param name="description">The optional document type description.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created document type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a document type with the same name already exists.</exception>
    Task<DocumentType> CreateAsync(string name, string? description, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing document type.
    /// </summary>
    /// <param name="id">The document type identifier.</param>
    /// <param name="name">The new document type name.</param>
    /// <param name="description">The new document type description.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated document type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the document type is not found or a document type with the new name already exists.</exception>
    Task<DocumentType> UpdateAsync(long id, string name, string? description, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a document type by its identifier.
    /// </summary>
    /// <param name="id">The document type identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The document type if found; otherwise null.</returns>
    Task<DocumentType?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all document types.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all document types ordered by name.</returns>
    Task<IReadOnlyCollection<DocumentType>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a document type.
    /// </summary>
    /// <param name="id">The document type identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
