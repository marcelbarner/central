namespace Central.Domain.Webhooks;

/// <summary>
/// Defines the types of events that can trigger webhooks.
/// </summary>
public enum WebhookEventType
{
    /// <summary>
    /// Triggered when a document is added.
    /// </summary>
    DocumentAdded = 1,

    /// <summary>
    /// Triggered when a document is updated.
    /// </summary>
    DocumentUpdated = 2,

    /// <summary>
    /// Triggered when a document is deleted.
    /// </summary>
    DocumentDeleted = 3
}