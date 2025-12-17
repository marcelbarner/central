namespace Central.Server.Features.Webhooks;

public sealed record WebhookDto
{
    public required long Id { get; init; }
    public required string EventType { get; init; }
    public required string Url { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required DateTimeOffset Updated { get; init; }
}