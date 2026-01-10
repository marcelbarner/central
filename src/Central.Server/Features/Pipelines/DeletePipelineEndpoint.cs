using Central.Domain.Documents.Ports;

using FastEndpoints;

namespace Central.Server.Features.Pipelines;

public sealed class DeletePipelineEndpoint(IPipelineRepository repository)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/api/pipelines/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");
        await repository.DeleteAsync(id, ct);
        await Send.NoContentAsync(ct);
    }
}
