namespace Central.Domain.Documents.Ports;

/// <summary>
/// Repository for managing PipelineExecution entities.
/// </summary>
public interface IPipelineExecutionRepository
{
    /// <summary>
    /// Creates a new pipeline execution.
    /// </summary>
    /// <param name="execution">The pipeline execution to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created pipeline execution with generated ID.</returns>
    Task<PipelineExecution> CreateAsync(PipelineExecution execution, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a pipeline execution by its ID, including all task executions.
    /// </summary>
    /// <param name="id">The pipeline execution ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The pipeline execution if found, otherwise null.</returns>
    Task<PipelineExecution?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all pipeline executions for a specific document.
    /// </summary>
    /// <param name="documentId">The document ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of pipeline executions for the document.</returns>
    Task<IReadOnlyCollection<PipelineExecution>> GetByDocumentIdAsync(
        long documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing pipeline execution.
    /// </summary>
    /// <param name="execution">The pipeline execution with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated pipeline execution.</returns>
    Task<PipelineExecution> UpdateAsync(PipelineExecution execution, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all pipeline executions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all pipeline executions.</returns>
    Task<IReadOnlyCollection<PipelineExecution>> GetAllAsync(CancellationToken cancellationToken = default);
}
