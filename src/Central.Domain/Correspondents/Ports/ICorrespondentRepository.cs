namespace Central.Domain.Correspondents.Ports;

/// <summary>
/// Port for managing correspondent persistence.
/// </summary>
public interface ICorrespondentRepository
{
    /// <summary>
    /// Adds a new correspondent to the repository.
    /// </summary>
    /// <param name="correspondent">The correspondent to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added correspondent with generated ID.</returns>
    Task<Correspondent> AddAsync(Correspondent correspondent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing correspondent.
    /// </summary>
    /// <param name="correspondent">The correspondent with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated correspondent.</returns>
    Task<Correspondent> UpdateAsync(Correspondent correspondent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a correspondent by its identifier.
    /// </summary>
    /// <param name="id">The correspondent identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The correspondent if found; otherwise null.</returns>
    Task<Correspondent?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a correspondent by its name.
    /// </summary>
    /// <param name="name">The correspondent name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The correspondent if found; otherwise null.</returns>
    Task<Correspondent?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

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

    /// <summary>
    /// Checks if a correspondent with the given name exists.
    /// </summary>
    /// <param name="name">The correspondent name to check.</param>
    /// <param name="excludeId">Optional correspondent ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a correspondent with the name exists; otherwise false.</returns>
    Task<bool> ExistsAsync(string name, long? excludeId = null, CancellationToken cancellationToken = default);
}
