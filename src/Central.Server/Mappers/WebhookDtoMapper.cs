using Central.Domain.Webhooks;
using Central.Server.Features.Webhooks;
using Riok.Mapperly.Abstractions;

namespace Central.Server.Mappers;

[Mapper]
public static partial class WebhookDtoMapper
{
    public static partial WebhookDto ToDto(this Webhook webhook);

    private static string MapEventType(WebhookEventType eventType) => eventType.ToString();
}
