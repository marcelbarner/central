namespace Central.Domain.Documents;

/// <summary>
/// Represents the processing state of a document.
/// </summary>
public enum DocumentState
{
    /// <summary>
    /// Document has been imported into the system.
    /// </summary>
    Imported = 0,

    /// <summary>
    /// Document is currently being processed.
    /// </summary>
    Processing = 1,

    /// <summary>
    /// Document is under review.
    /// </summary>
    Review = 2,

    /// <summary>
    /// Document has been approved.
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Document processing or review has failed.
    /// </summary>
    Failed = 4
}
