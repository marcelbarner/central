namespace Central.Domain.Documents;

/// <summary>
/// Represents a reusable AI processing task that can be executed independently or as part of a pipeline.
/// </summary>
public sealed record ProcessingTask
{
    /// <summary>
    /// Gets the unique identifier for the task.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Gets the user-friendly name of the task.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the description of what this task does.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the type of AI task (AzureOpenAI or AzureDocumentIntelligence).
    /// </summary>
    public required TaskType TaskType { get; init; }

    /// <summary>
    /// Gets the task configuration.
    /// </summary>
    public required TaskConfiguration Configuration { get; init; }

    /// <summary>
    /// Gets a value indicating whether this task is enabled and can be executed.
    /// </summary>
    public required bool Enabled { get; init; }

    /// <summary>
    /// Gets the timestamp when the task was created.
    /// </summary>
    public required DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the timestamp when the task was last updated.
    /// </summary>
    public required DateTimeOffset Updated { get; init; }
}
