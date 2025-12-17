using Central.Domain.Webhooks.Services;

using FastEndpoints;

namespace Central.Server.Features.Webhooks;

public sealed record DeleteWebhookRequest
{
    public long Id { get; init; }
}

public sealed class DeleteWebhookEndpoint(IWebhookService webhookService)
    : Endpoint<DeleteWebhookRequest>
{
    public override void Configure()
    {
        Delete("/api/webhooks/{Id}");
    }

    public override async Task HandleAsync(DeleteWebhookRequest req, CancellationToken ct)
    {
        try
        {
            await webhookService.DeleteAsync(req.Id, ct);
            await Send.NoContentAsync(ct);
        }
        catch (InvalidOperationException)
        {
            await Send.NotFoundAsync(ct);
        }
    }
}