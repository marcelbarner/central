namespace Central.Domain.Documents;

/// <summary>
/// Represents a step within a pipeline workflow.
/// </summary>
public sealed record PipelineStep
{
    /// <summary>
    /// Gets the unique identifier for the step.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Gets the ID of the pipeline this step belongs to.
    /// </summary>
    public required long PipelineId { get; init; }

    /// <summary>
    /// Gets the user-friendly name of the step.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the type of pipeline step (TaskStep or WaitStep).
    /// </summary>
    public required PipelineStepType StepType { get; init; }

    /// <summary>
    /// Gets the execution order of this step within the pipeline (0-based).
    /// </summary>
    public required int Order { get; init; }

    /// <summary>
    /// Gets the ID of the task to execute (for TaskStep only).
    /// </summary>
    public long? TaskId { get; init; }

    /// <summary>
    /// Gets the wait duration in seconds (for WaitStep only).
    /// </summary>
    public int? WaitDurationSeconds { get; init; }
}
