namespace Central.Domain.Documents;

/// <summary>
/// Represents the type of AI task that can be executed on documents.
/// </summary>
public enum TaskType
{
    /// <summary>
    /// Azure OpenAI task that uses GPT models to analyze and enrich documents.
    /// </summary>
    AzureOpenAI,

    /// <summary>
    /// Azure Document Intelligence task that extracts content from documents.
    /// </summary>
    AzureDocumentIntelligence
}
