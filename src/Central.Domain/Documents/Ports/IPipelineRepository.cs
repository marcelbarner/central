namespace Central.Domain.Documents.Ports;

/// <summary>
/// Repository for managing Pipeline aggregate roots.
/// </summary>
public interface IPipelineRepository
{
    /// <summary>
    /// Creates a new pipeline.
    /// </summary>
    /// <param name="pipeline">The pipeline to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created pipeline with generated ID.</returns>
    Task<Pipeline> CreateAsync(Pipeline pipeline, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a pipeline by its ID.
    /// </summary>
    /// <param name="id">The pipeline ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The pipeline if found, otherwise null.</returns>
    Task<Pipeline?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all pipelines.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all pipelines.</returns>
    Task<IReadOnlyCollection<Pipeline>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing pipeline.
    /// </summary>
    /// <param name="pipeline">The pipeline with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated pipeline.</returns>
    Task<Pipeline> UpdateAsync(Pipeline pipeline, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a pipeline by its ID.
    /// </summary>
    /// <param name="id">The pipeline ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all enabled pipelines that should trigger when documents reach the specified state.
    /// </summary>
    /// <param name="triggerState">The document state that triggers pipeline execution.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of enabled pipelines matching the trigger state.</returns>
    Task<IReadOnlyCollection<Pipeline>> GetEnabledByTriggerStateAsync(
        DocumentState triggerState,
        CancellationToken cancellationToken = default);
}
