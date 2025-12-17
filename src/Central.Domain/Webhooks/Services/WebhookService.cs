using Central.Domain.Webhooks.Ports;

namespace Central.Domain.Webhooks.Services;

/// <summary>
/// Domain service for managing webhooks.
/// </summary>
public class WebhookService(IWebhookRepository repository) : IWebhookService
{
    /// <inheritdoc />
    public async Task<Webhook> CreateAsync(WebhookEventType eventType, string url, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != "http" && uri.Scheme != "https"))
        {
            throw new ArgumentException("URL must be a valid HTTP or HTTPS URL.", nameof(url));
        }

        var webhook = new Webhook
        {
            EventType = eventType,
            Url = url,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow
        };

        return await repository.AddAsync(webhook, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Webhook> UpdateAsync(long id, WebhookEventType eventType, string url, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != "http" && uri.Scheme != "https"))
        {
            throw new ArgumentException("URL must be a valid HTTP or HTTPS URL.", nameof(url));
        }

        var existing = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Webhook with ID {id} not found.");

        var updated = existing with
        {
            EventType = eventType,
            Url = url,
            Updated = DateTimeOffset.UtcNow
        };

        return await repository.UpdateAsync(updated, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Webhook?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return repository.GetByIdAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<Webhook>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetAllAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Webhook with ID {id} not found.");

        await repository.DeleteAsync(id, cancellationToken);
    }
}