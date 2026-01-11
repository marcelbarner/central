namespace Central.Domain.Ports;

/// <summary>
/// Execution context containing all information needed to execute a task.
/// </summary>
public sealed record TaskExecutionContext
{
    /// <summary>
    /// The task to execute.
    /// </summary>
    public required Documents.ProcessingTask Task { get; init; }

    /// <summary>
    /// The document to process.
    /// </summary>
    public required Documents.Document Document { get; init; }

    /// <summary>
    /// Optional pipeline execution ID if this task is part of a pipeline.
    /// </summary>
    public long? PipelineExecutionId { get; init; }

    /// <summary>
    /// Optional wait duration in seconds for wait tasks.
    /// </summary>
    public int? WaitDurationSeconds { get; init; }
}
