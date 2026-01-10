namespace Central.Domain.Documents;

/// <summary>
/// Represents a configured pipeline that orchestrates document processing through multiple steps.
/// </summary>
public sealed record Pipeline
{
    /// <summary>
    /// Gets the unique identifier for the pipeline.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Gets the user-friendly name of the pipeline.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the description of what this pipeline does.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets a value indicating whether this pipeline is active and should execute automatically.
    /// </summary>
    public required bool Enabled { get; init; }

    /// <summary>
    /// Gets the document state that triggers this pipeline to execute automatically.
    /// When null, the pipeline can only be executed manually.
    /// </summary>
    public DocumentState? TriggerState { get; init; }

    /// <summary>
    /// Gets the timestamp when the pipeline was created.
    /// </summary>
    public required DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the timestamp when the pipeline was last updated.
    /// </summary>
    public required DateTimeOffset Updated { get; init; }

    /// <summary>
    /// Gets the collection of pipeline steps in execution order.
    /// </summary>
    public IReadOnlyCollection<PipelineStep> Steps { get; init; } = Array.Empty<PipelineStep>();
}
