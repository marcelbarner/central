namespace Central.Domain.Documents.Ports;

/// <summary>
/// Repository for managing process execution persistence and querying.
/// </summary>
public interface IProcessExecutionRepository
{
    /// <summary>
    /// Gets a process execution by its unique identifier.
    /// </summary>
    /// <param name="id">The process execution ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The process execution, or null if not found.</returns>
    Task<ProcessExecution?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all process executions for a specific document.
    /// </summary>
    /// <param name="documentId">The document ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of process executions for the document, ordered by started date descending.</returns>
    Task<IReadOnlyCollection<ProcessExecution>> GetByDocumentIdAsync(
        long documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all process executions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all process executions, ordered by started date descending.</returns>
    Task<IReadOnlyCollection<ProcessExecution>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all process executions with a specific status.
    /// </summary>
    /// <param name="status">The execution status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of process executions with the specified status.</returns>
    Task<IReadOnlyCollection<ProcessExecution>> GetByStatusAsync(
        ExecutionStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new process execution.
    /// </summary>
    /// <param name="processExecution">The process execution to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created process execution with generated ID.</returns>
    Task<ProcessExecution> CreateAsync(ProcessExecution processExecution, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing process execution.
    /// </summary>
    /// <param name="processExecution">The process execution to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated process execution.</returns>
    Task<ProcessExecution> UpdateAsync(ProcessExecution processExecution, CancellationToken cancellationToken = default);
}