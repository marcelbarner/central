namespace Central.Domain.Tags.Services;

/// <summary>
/// Domain service for managing tag operations.
/// </summary>
public interface ITagService
{
    /// <summary>
    /// Creates a new tag.
    /// </summary>
    /// <param name="name">The tag name.</param>
    /// <param name="description">The optional tag description.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created tag.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a tag with the same name already exists.</exception>
    Task<Tag> CreateAsync(string name, string? description, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing tag.
    /// </summary>
    /// <param name="id">The tag identifier.</param>
    /// <param name="name">The new tag name.</param>
    /// <param name="description">The new tag description.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated tag.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the tag is not found or a tag with the new name already exists.</exception>
    Task<Tag> UpdateAsync(long id, string name, string? description, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a tag by its identifier.
    /// </summary>
    /// <param name="id">The tag identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tag if found; otherwise null.</returns>
    Task<Tag?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tags.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all tags ordered by name.</returns>
    Task<IReadOnlyCollection<Tag>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a tag.
    /// </summary>
    /// <param name="id">The tag identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}