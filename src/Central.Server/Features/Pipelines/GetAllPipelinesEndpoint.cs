using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Pipelines;

public sealed class GetAllPipelinesEndpoint(IPipelineRepository repository)
    : EndpointWithoutRequest<IReadOnlyCollection<PipelineDto>>
{
    public override void Configure()
    {
        Get("/api/pipelines");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var pipelines = await repository.GetAllAsync(ct);
        var dtos = pipelines.Select(p => p.ToDto()).ToList();

        await Send.OkAsync(dtos, ct);
    }
}
