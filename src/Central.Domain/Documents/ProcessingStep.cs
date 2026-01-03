namespace Central.Domain.Documents;

/// <summary>
/// Represents a configured step within a process definition.
/// </summary>
public sealed record ProcessingStep
{
    /// <summary>
    /// Gets the unique identifier for the step.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Gets the ID of the process definition this step belongs to.
    /// </summary>
    public required long ProcessDefinitionId { get; init; }

    /// <summary>
    /// Gets the user-friendly name of the step.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the description of what this step does.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the type of processing step (AzureOpenAI or AzureDocumentIntelligence).
    /// </summary>
    public required StepType StepType { get; init; }

    /// <summary>
    /// Gets the execution order of this step within the process (0-based).
    /// </summary>
    public required int Order { get; init; }

    /// <summary>
    /// Gets the Azure endpoint URL for this step's AI service.
    /// </summary>
    public string? AzureEndpoint { get; init; }

    /// <summary>
    /// Gets the Azure API key for this step's AI service.
    /// </summary>
    public string? AzureApiKey { get; init; }

    /// <summary>
    /// Gets the Azure OpenAI deployment name (for OpenAI steps) or Document Intelligence model ID (for DI steps).
    /// </summary>
    public string? AzureModelOrDeployment { get; init; }

    /// <summary>
    /// Gets the AI prompt used for this step (for OpenAI steps).
    /// </summary>
    public string? Prompt { get; init; }

    /// <summary>
    /// Gets additional configuration specific to the step type.
    /// For AzureOpenAI: JSON array of enabled capabilities (e.g., ["SetDocumentTitle", "GetDocumentTypes"])
    /// For AzureDocumentIntelligence: JSON object with settings (e.g., {"outputType": "markdown"})
    /// </summary>
    public string? Configuration { get; init; }
}