namespace Central.Domain.Tags.Ports;

/// <summary>
/// Port for managing tag persistence.
/// </summary>
public interface ITagRepository
{
    /// <summary>
    /// Adds a new tag to the repository.
    /// </summary>
    /// <param name="tag">The tag to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added tag with generated ID.</returns>
    Task<Tag> AddAsync(Tag tag, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing tag.
    /// </summary>
    /// <param name="tag">The tag with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated tag.</returns>
    Task<Tag> UpdateAsync(Tag tag, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a tag by its identifier.
    /// </summary>
    /// <param name="id">The tag identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tag if found; otherwise null.</returns>
    Task<Tag?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a tag by its name.
    /// </summary>
    /// <param name="name">The tag name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tag if found; otherwise null.</returns>
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tags.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all tags.</returns>
    Task<IReadOnlyCollection<Tag>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a tag by its identifier.
    /// </summary>
    /// <param name="id">The tag identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a tag with the given name exists.
    /// </summary>
    /// <param name="name">The tag name to check.</param>
    /// <param name="excludeId">Optional tag ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a tag with the name exists; otherwise false.</returns>
    Task<bool> ExistsAsync(string name, long? excludeId = null, CancellationToken cancellationToken = default);
}