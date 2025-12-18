using Central.Domain.Webhooks;
using Central.Domain.Webhooks.Services;
using Central.Server.Mappers;

using FastEndpoints;

using FluentValidation;

namespace Central.Server.Features.Webhooks;

public sealed record CreateWebhookRequest
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public required string EventType { get; init; }
    public required string Url { get; init; }

    internal sealed class Validator : Validator<CreateWebhookRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name).MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(1000);

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

public sealed class CreateWebhookEndpoint(IWebhookService webhookService)
    : Endpoint<CreateWebhookRequest, WebhookDto>
{
    public override void Configure()
    {
        Post("/api/webhooks");
    }

    public override async Task HandleAsync(CreateWebhookRequest req, CancellationToken ct)
    {
        try
        {
            var eventType = Enum.Parse<WebhookEventType>(req.EventType, true);
            var webhook = await webhookService.CreateAsync(eventType, req.Url, req.Name, req.Description, ct);
            await Send.CreatedAtAsync<GetWebhookByIdEndpoint>(
                new { webhook.Id },
                webhook.ToDto(),
                cancellation: ct);
        }
        catch (ArgumentException ex)
        {
            ThrowError(ex.Message);
        }
    }
}