namespace Central.Domain.Documents;

/// <summary>
/// Represents the result of executing a single step within a process execution.
/// </summary>
public sealed record ProcessExecutionStep
{
    /// <summary>
    /// Gets the unique identifier for this step execution.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Gets the ID of the process execution this step belongs to.
    /// </summary>
    public required long ProcessExecutionId { get; init; }

    /// <summary>
    /// Gets the name of the step that was executed.
    /// </summary>
    public required string StepName { get; init; }

    /// <summary>
    /// Gets the type of step that was executed.
    /// </summary>
    public required StepType StepType { get; init; }

    /// <summary>
    /// Gets the execution order of this step.
    /// </summary>
    public required int Order { get; init; }

    /// <summary>
    /// Gets the execution status of this step.
    /// </summary>
    public required ExecutionStatus Status { get; init; }

    /// <summary>
    /// Gets the timestamp when the step started executing.
    /// </summary>
    public DateTimeOffset? StartedAt { get; init; }

    /// <summary>
    /// Gets the timestamp when the step completed or failed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; init; }

    /// <summary>
    /// Gets the error message if the step failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets the output or result data from the step execution (e.g., extracted content, AI response).
    /// </summary>
    public string? Output { get; init; }
}