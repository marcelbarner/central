namespace Central.Domain.Webhooks.Ports;

/// <summary>
/// Port for triggering webhooks.
/// </summary>
public interface IWebhookTrigger
{
    /// <summary>
    /// Triggers all webhooks subscribed to a specific event type.
    /// </summary>
    /// <param name="eventType">The type of event that occurred.</param>
    /// <param name="documentId">The ID of the document that triggered the event.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task TriggerAsync(WebhookEventType eventType, long documentId, CancellationToken cancellationToken = default);
}