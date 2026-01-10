namespace Central.Domain.Documents;

/// <summary>
/// Represents a runtime execution instance of a task against a specific document.
/// </summary>
public sealed record TaskExecution
{
    /// <summary>
    /// Gets the unique identifier for this task execution.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Gets the ID of the task being executed.
    /// </summary>
    public required long TaskId { get; init; }

    /// <summary>
    /// Gets the ID of the document being processed.
    /// </summary>
    public required long DocumentId { get; init; }

    /// <summary>
    /// Gets the ID of the pipeline execution if this task was executed as part of a pipeline.
    /// Null for direct task executions.
    /// </summary>
    public long? PipelineExecutionId { get; init; }

    /// <summary>
    /// Gets the execution status.
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
    /// Gets the JSON result from the AI service.
    /// </summary>
    public string? Result { get; init; }
}
