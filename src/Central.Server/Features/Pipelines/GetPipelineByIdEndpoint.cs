using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Pipelines;

public sealed class GetPipelineByIdEndpoint(IPipelineRepository repository)
    : EndpointWithoutRequest<PipelineDto>
{
    public override void Configure()
    {
        Get("/api/pipelines/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");
        var pipeline = await repository.GetByIdAsync(id, ct);

        if (pipeline == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(pipeline.ToDto(), ct);
    }
}
