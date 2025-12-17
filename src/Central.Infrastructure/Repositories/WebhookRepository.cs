using Central.Domain.Webhooks;
using Central.Domain.Webhooks.Ports;
using Central.Infrastructure.Mappers;
using Central.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for webhooks.
/// </summary>
public class WebhookRepository(ApplicationDbContext context) : IWebhookRepository
{
    /// <inheritdoc />
    public async Task<Webhook> AddAsync(Webhook webhook, CancellationToken cancellationToken = default)
    {
        var entity = webhook.ToEntity();
        await context.Webhooks.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entity.ToDomain();
    }

    /// <inheritdoc />
    public async Task<Webhook> UpdateAsync(Webhook webhook, CancellationToken cancellationToken = default)
    {
        var entity = webhook.ToEntity();
        context.Webhooks.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity.ToDomain();
    }

    /// <inheritdoc />
    public async Task<Webhook?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Webhooks
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        return entity?.ToDomain();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Webhook>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context.Webhooks
            .AsNoTracking()
            .OrderBy(w => w.Created)
            .ToListAsync(cancellationToken);
        return entities.Select(e => e.ToDomain()).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Webhook>> GetByEventTypeAsync(WebhookEventType eventType, CancellationToken cancellationToken = default)
    {
        var eventTypeValue = (int)eventType;
        var entities = await context.Webhooks
            .AsNoTracking()
            .Where(w => w.EventType == eventTypeValue)
            .ToListAsync(cancellationToken);
        return entities.Select(e => e.ToDomain()).ToList();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Webhooks.FindAsync([id], cancellationToken);
        if (entity != null)
        {
            context.Webhooks.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
