namespace Central.Domain.Webhooks;

/// <summary>
/// Represents a webhook subscription for document events.
/// </summary>
public sealed record Webhook
{
    /// <summary>
    /// Gets the unique identifier for the webhook.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Gets the type of event this webhook subscribes to.
    /// </summary>
    public required WebhookEventType EventType { get; init; }

    /// <summary>
    /// Gets the URL where the webhook POST request will be sent.
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// Gets the timestamp when the webhook was created.
    /// </summary>
    public required DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the timestamp when the webhook was last updated.
    /// </summary>
    public required DateTimeOffset Updated { get; init; }
}
