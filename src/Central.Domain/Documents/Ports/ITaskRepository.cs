namespace Central.Domain.Documents.Ports;

/// <summary>
/// Repository for managing ProcessingTask aggregate roots.
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="task">The task to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created task with generated ID.</returns>
    System.Threading.Tasks.Task<ProcessingTask> CreateAsync(ProcessingTask task, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a task by its ID.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task if found, otherwise null.</returns>
    System.Threading.Tasks.Task<ProcessingTask?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tasks.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all tasks.</returns>
    System.Threading.Tasks.Task<IReadOnlyCollection<ProcessingTask>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="task">The task with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated task.</returns>
    System.Threading.Tasks.Task<ProcessingTask> UpdateAsync(ProcessingTask task, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a task by its ID.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all enabled tasks.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of enabled tasks.</returns>
    System.Threading.Tasks.Task<IReadOnlyCollection<ProcessingTask>> GetEnabledAsync(CancellationToken cancellationToken = default);
}
