namespace Central.Domain.Documents;

/// <summary>
/// Represents a runtime execution instance of a process definition against a specific document.
/// </summary>
public sealed record ProcessExecution
{
    /// <summary>
    /// Gets the unique identifier for this process execution.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Gets the ID of the process definition being executed.
    /// </summary>
    public required long ProcessDefinitionId { get; init; }

    /// <summary>
    /// Gets the ID of the document being processed.
    /// </summary>
    public required long DocumentId { get; init; }

    /// <summary>
    /// Gets the overall execution status.
    /// </summary>
    public required ExecutionStatus Status { get; init; }

    /// <summary>
    /// Gets the timestamp when the execution started.
    /// </summary>
    public DateTimeOffset? StartedAt { get; init; }

    /// <summary>
    /// Gets the timestamp when the execution completed, failed, or was cancelled.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; init; }

    /// <summary>
    /// Gets the error message if the execution failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets the collection of step execution results.
    /// </summary>
    public IReadOnlyCollection<ProcessExecutionStep> Steps { get; init; } = Array.Empty<ProcessExecutionStep>();
}