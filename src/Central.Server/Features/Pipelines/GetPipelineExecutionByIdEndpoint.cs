using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Pipelines;

public sealed class GetPipelineExecutionByIdEndpoint(IPipelineExecutionRepository repository)
    : EndpointWithoutRequest<PipelineExecutionDto>
{
    public override void Configure()
    {
        Get("/api/pipeline-executions/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");
        var execution = await repository.GetByIdAsync(id, ct);

        if (execution == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(execution.ToDto(), ct);
    }
}
