namespace Central.Domain.Documents;

/// <summary>
/// Configuration for an AI task execution.
/// </summary>
public sealed record TaskConfiguration
{
    /// <summary>
    /// Gets the Azure endpoint URL for the AI service.
    /// </summary>
    public string? AzureEndpoint { get; init; }

    /// <summary>
    /// Gets the Azure API key for authentication.
    /// </summary>
    public string? AzureApiKey { get; init; }

    /// <summary>
    /// Gets the Azure OpenAI deployment name or Document Intelligence model ID.
    /// </summary>
    public string? AzureModelOrDeployment { get; init; }

    /// <summary>
    /// Gets the AI prompt used for OpenAI tasks.
    /// </summary>
    public string? Prompt { get; init; }

    /// <summary>
    /// Gets the sampling temperature for OpenAI tasks (0.0 to 2.0).
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    /// Gets the maximum number of tokens for OpenAI responses.
    /// </summary>
    public int? MaxTokens { get; init; }

    /// <summary>
    /// Gets enabled capabilities for OpenAI tasks (JSON array).
    /// Example: ["SetDocumentTitle", "GetDocumentTypes", "AddTags"]
    /// </summary>
    public string? Capabilities { get; init; }

    /// <summary>
    /// Gets Document Intelligence-specific options (JSON object).
    /// </summary>
    public string? DocumentIntelligenceOptions { get; init; }
}
