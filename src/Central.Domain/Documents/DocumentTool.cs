namespace Central.Domain.Documents;

/// <summary>
/// Available tools that can be used by AI tasks to interact with documents and entities.
/// </summary>
public enum DocumentTool
{
    /// <summary>
    /// Allows the AI to update the document title based on content analysis.
    /// </summary>
    SetTitle,

    /// <summary>
    /// Allows the AI to set the document date based on content analysis.
    /// </summary>
    SetDate,

    /// <summary>
    /// Allows the AI to assign a contract to the document.
    /// </summary>
    SetContract,

    /// <summary>
    /// Allows the AI to assign a correspondent to the document.
    /// </summary>
    SetCorrespondent,

    /// <summary>
    /// Allows the AI to classify the document by type.
    /// </summary>
    SetDocumentType,

    /// <summary>
    /// Allows the AI to assign tags to the document.
    /// </summary>
    SetTags,

    /// <summary>
    /// Allows the AI to update the document's text content.
    /// </summary>
    SetContent,

    /// <summary>
    /// Allows the AI to access the content of the current document for analysis.
    /// </summary>
    GetContent,

    /// <summary>
    /// Allows the AI to retrieve detailed information about a specific document.
    /// </summary>
    GetDocument,

    /// <summary>
    /// Provides the AI with examples of existing document titles for consistency.
    /// </summary>
    GetSimilar,

    /// <summary>
    /// Provides the AI with a list of available contracts.
    /// </summary>
    GetContracts,

    /// <summary>
    /// Provides the AI with a list of available document types.
    /// </summary>
    GetDocumentTypes,

    /// <summary>
    /// Provides the AI with a list of available correspondents.
    /// </summary>
    GetCorrespondents,

    /// <summary>
    /// Provides the AI with a list of available tags.
    /// </summary>
    GetTags,

    /// <summary>
    /// Allows the AI to create new contracts.
    /// </summary>
    CreateContract,

    /// <summary>
    /// Allows the AI to create new correspondents.
    /// </summary>
    CreateCorrespondent,

    /// <summary>
    /// Allows the AI to create new document types.
    /// </summary>
    CreateDocumentType,

    /// <summary>
    /// Allows the AI to create new tags.
    /// </summary>
    CreateTag
}
