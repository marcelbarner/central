using Central.Domain.Webhooks;
using Central.Infrastructure.Entities;

using Riok.Mapperly.Abstractions;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Mapper for webhook entities.
/// </summary>
[Mapper]
public static partial class WebhookMapper
{
    /// <summary>
    /// Maps a webhook entity to a domain webhook.
    /// </summary>
    public static partial Webhook ToDomain(this WebhookEntity entity);

    /// <summary>
    /// Maps a domain webhook to a webhook entity.
    /// </summary>
    public static partial WebhookEntity ToEntity(this Webhook webhook);

    /// <summary>
    /// Maps WebhookEventType enum to int.
    /// </summary>
    private static int MapEventType(WebhookEventType eventType) => (int)eventType;

    /// <summary>
    /// Maps int to WebhookEventType enum.
    /// </summary>
    private static WebhookEventType MapEventType(int eventType) => (WebhookEventType)eventType;
}