namespace Central.Domain.Documents;

/// <summary>
/// Represents the type of processing step in a document processing workflow.
/// </summary>
public enum StepType
{
    /// <summary>
    /// Azure OpenAI step that uses GPT models to analyze and enrich documents.
    /// </summary>
    AzureOpenAI = 0,

    /// <summary>
    /// Azure Document Intelligence step that extracts content from documents.
    /// </summary>
    AzureDocumentIntelligence = 1
}