namespace Central.Domain.Webhooks;

/// <summary>
/// Represents the payload sent to webhook endpoints.
/// </summary>
public sealed record WebhookPayload
{
    /// <summary>
    /// Gets the type of event that triggered this webhook.
    /// </summary>
    public required string EventType { get; init; }

    /// <summary>
    /// Gets the ID of the document that triggered this webhook.
    /// </summary>
    public required long DocumentId { get; init; }

    /// <summary>
    /// Gets the timestamp when the event occurred.
    /// </summary>
    public required DateTimeOffset Timestamp { get; init; }
}