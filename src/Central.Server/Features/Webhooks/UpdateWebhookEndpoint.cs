using Central.Domain.Webhooks;
using Central.Domain.Webhooks.Services;
using Central.Server.Mappers;
using FastEndpoints;
using FluentValidation;

namespace Central.Server.Features.Webhooks;

public sealed record UpdateWebhookRequest
{
    public required long Id { get; init; }
    public required string EventType { get; init; }
    public required string Url { get; init; }

    internal sealed class Validator : Validator<UpdateWebhookRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            
            RuleFor(x => x.EventType)
                .NotEmpty()
                .Must(BeValidEventType)
                .WithMessage("EventType must be one of: DocumentAdded, DocumentUpdated, DocumentDeleted");
            
            RuleFor(x => x.Url)
                .NotEmpty()
                .Must(BeValidUrl)
                .WithMessage("Url must be a valid HTTP or HTTPS URL");
        }

        private bool BeValidEventType(string eventType)
        {
            return Enum.TryParse<WebhookEventType>(eventType, true, out _);
        }

        private bool BeValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri) && 
                   (uri.Scheme == "http" || uri.Scheme == "https");
        }
    }
}

public sealed class UpdateWebhookEndpoint(IWebhookService webhookService)
    : Endpoint<UpdateWebhookRequest, WebhookDto>
{
    public override void Configure()
    {
        Put("/api/webhooks/{Id}");
    }

    public override async Task HandleAsync(UpdateWebhookRequest req, CancellationToken ct)
    {
        try
        {
            var eventType = Enum.Parse<WebhookEventType>(req.EventType, true);
            var webhook = await webhookService.UpdateAsync(req.Id, eventType, req.Url, ct);
            await Send.OkAsync(webhook.ToDto(), ct);
        }
        catch (InvalidOperationException)
        {
            await Send.NotFoundAsync(ct);
        }
        catch (ArgumentException ex)
        {
            ThrowError(ex.Message);
        }
    }
}
