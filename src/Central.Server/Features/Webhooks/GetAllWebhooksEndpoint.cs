using Central.Domain.Webhooks.Services;
using Central.Server.Mappers;
using FastEndpoints;

namespace Central.Server.Features.Webhooks;

public sealed class GetAllWebhooksEndpoint(IWebhookService webhookService)
    : EndpointWithoutRequest<IReadOnlyCollection<WebhookDto>>
{
    public override void Configure()
    {
        Get("/api/webhooks");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var webhooks = await webhookService.GetAllAsync(ct);
        var dtos = webhooks.Select(w => w.ToDto()).ToList();
        await Send.OkAsync(dtos, ct);
    }
}
