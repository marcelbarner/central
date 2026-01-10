namespace Central.Domain.Documents.Ports;

/// <summary>
/// Repository for managing TaskExecution entities.
/// </summary>
public interface ITaskExecutionRepository
{
    /// <summary>
    /// Creates a new task execution.
    /// </summary>
    /// <param name="execution">The task execution to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created task execution with generated ID.</returns>
    Task<TaskExecution> CreateAsync(TaskExecution execution, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a task execution by its ID.
    /// </summary>
    /// <param name="id">The task execution ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task execution if found, otherwise null.</returns>
    Task<TaskExecution?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all task executions for a specific document.
    /// </summary>
    /// <param name="documentId">The document ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of task executions for the document.</returns>
    Task<IReadOnlyCollection<TaskExecution>> GetByDocumentIdAsync(
        long documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all task executions for a specific task.
    /// </summary>
    /// <param name="taskId">The task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of task executions for the task.</returns>
    Task<IReadOnlyCollection<TaskExecution>> GetByTaskIdAsync(
        long taskId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing task execution.
    /// </summary>
    /// <param name="execution">The task execution with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated task execution.</returns>
    Task<TaskExecution> UpdateAsync(TaskExecution execution, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all task executions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all task executions.</returns>
    Task<IReadOnlyCollection<TaskExecution>> GetAllAsync(CancellationToken cancellationToken = default);
}
