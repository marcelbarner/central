using Central.Domain.Documents.Services;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Pipelines;

public sealed class ExecutePipelineEndpoint(PipelineExecutionService pipelineExecutionService)
    : Endpoint<ExecutePipelineRequest, PipelineExecutionDto>
{
    public override void Configure()
    {
        Post("/api/pipelines/{id}/execute");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ExecutePipelineRequest req, CancellationToken ct)
    {
        var pipelineId = Route<long>("id");
        var execution = await pipelineExecutionService.ExecutePipelineAsync(pipelineId, req.DocumentId, ct);

        await Send.CreatedAtAsync<GetPipelineExecutionByIdEndpoint>(
            new { id = execution.Id },
            execution.ToDto(),
            cancellation: ct);
    }
}
