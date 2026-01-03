using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.ProcessExecutions;

public sealed class GetProcessExecutionByIdEndpoint(IProcessExecutionRepository repository)
    : EndpointWithoutRequest<ProcessExecutionDto>
{
    public override void Configure()
    {
        Get("/api/process-executions/{id}");
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

        var dto = execution.ToDto();
        await Send.OkAsync(dto, ct);
    }
}