namespace Central.Domain.Ports;

/// <summary>
/// Interface for executing a specific type of task.
/// </summary>
public interface ITaskExecuter
{
    /// <summary>
    /// Executes a task with the provided context.
    /// </summary>
    /// <param name="context">The execution context containing task, document and configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The execution result as a string.</returns>
    Task<string> ExecuteAsync(TaskExecutionContext context, CancellationToken cancellationToken = default);
}
