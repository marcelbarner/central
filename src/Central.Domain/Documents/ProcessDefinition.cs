namespace Central.Domain.Documents;

/// <summary>
/// Represents a configured process definition that orchestrates document processing through multiple steps.
/// </summary>
public sealed record ProcessDefinition
{
    /// <summary>
    /// Gets the unique identifier for the process definition.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Gets the user-friendly name of the process.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the description of what this process does.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets a value indicating whether this process is active and should execute automatically.
    /// </summary>
    public required bool Enabled { get; init; }

    /// <summary>
    /// Gets the document state that triggers this process to execute.
    /// </summary>
    public required DocumentState TriggerState { get; init; }

    /// <summary>
    /// Gets the timestamp when the process was created.
    /// </summary>
    public required DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the timestamp when the process was last updated.
    /// </summary>
    public required DateTimeOffset Updated { get; init; }

    /// <summary>
    /// Gets the collection of processing steps in execution order.
    /// </summary>
    public IReadOnlyCollection<ProcessingStep> Steps { get; init; } = Array.Empty<ProcessingStep>();
}