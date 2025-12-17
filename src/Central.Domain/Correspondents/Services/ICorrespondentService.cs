namespace Central.Domain.Correspondents.Services;

/// <summary>
/// Service for managing correspondents.
/// </summary>
public interface ICorrespondentService
{
    /// <summary>
    /// Creates a new correspondent.
    /// </summary>
    /// <param name="name">The correspondent name.</param>
    /// <param name="description">The correspondent description.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created correspondent.</returns>
    Task<Correspondent> CreateAsync(string name, string? description = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing correspondent.
    /// </summary>
    /// <param name="id">The correspondent identifier.</param>
    /// <param name="name">The correspondent name.</param>
    /// <param name="description">The correspondent description.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated correspondent.</returns>
    Task<Correspondent> UpdateAsync(long id, string name, string? description = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a correspondent by its identifier.
    /// </summary>
    /// <param name="id">The correspondent identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The correspondent if found; otherwise null.</returns>
    Task<Correspondent?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all correspondents.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all correspondents.</returns>
    Task<IReadOnlyCollection<Correspondent>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a correspondent by its identifier.
    /// </summary>
    /// <param name="id">The correspondent identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
