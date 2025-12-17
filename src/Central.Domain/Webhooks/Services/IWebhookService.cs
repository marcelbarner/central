namespace Central.Domain.Webhooks.Services;

/// <summary>
/// Service for managing webhooks.
/// </summary>
public interface IWebhookService
{
    /// <summary>
    /// Creates a new webhook.
    /// </summary>
    /// <param name="eventType">The event type to subscribe to.</param>
    /// <param name="url">The URL where the webhook POST request will be sent.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created webhook.</returns>
    Task<Webhook> CreateAsync(WebhookEventType eventType, string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing webhook.
    /// </summary>
    /// <param name="id">The webhook identifier.</param>
    /// <param name="eventType">The event type to subscribe to.</param>
    /// <param name="url">The URL where the webhook POST request will be sent.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated webhook.</returns>
    Task<Webhook> UpdateAsync(long id, WebhookEventType eventType, string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a webhook by its identifier.
    /// </summary>
    /// <param name="id">The webhook identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The webhook if found; otherwise null.</returns>
    Task<Webhook?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all webhooks.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all webhooks.</returns>
    Task<IReadOnlyCollection<Webhook>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a webhook by its identifier.
    /// </summary>
    /// <param name="id">The webhook identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
