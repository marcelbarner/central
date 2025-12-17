namespace Central.Domain.Webhooks.Ports;

/// <summary>
/// Port for managing webhook persistence.
/// </summary>
public interface IWebhookRepository
{
    /// <summary>
    /// Adds a new webhook to the repository.
    /// </summary>
    /// <param name="webhook">The webhook to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added webhook with generated ID.</returns>
    Task<Webhook> AddAsync(Webhook webhook, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing webhook.
    /// </summary>
    /// <param name="webhook">The webhook with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated webhook.</returns>
    Task<Webhook> UpdateAsync(Webhook webhook, CancellationToken cancellationToken = default);

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
    /// Gets all webhooks subscribed to a specific event type.
    /// </summary>
    /// <param name="eventType">The event type to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of webhooks for the specified event type.</returns>
    Task<IReadOnlyCollection<Webhook>> GetByEventTypeAsync(WebhookEventType eventType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a webhook by its identifier.
    /// </summary>
    /// <param name="id">The webhook identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}