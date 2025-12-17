using Central.Domain.Webhooks.Services;
using Central.Server.Mappers;
using FastEndpoints;

namespace Central.Server.Features.Webhooks;

public sealed record GetWebhookByIdRequest
{
    public long Id { get; init; }
}

public sealed class GetWebhookByIdEndpoint(IWebhookService webhookService)
    : Endpoint<GetWebhookByIdRequest, WebhookDto>
{
    public override void Configure()
    {
        Get("/api/webhooks/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetWebhookByIdRequest req, CancellationToken ct)
    {
        var webhook = await webhookService.GetByIdAsync(req.Id, ct);
        if (webhook == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(webhook.ToDto(), ct);
    }
}
