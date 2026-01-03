using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.ProcessExecutions;

public sealed class GetAllProcessExecutionsEndpoint(IProcessExecutionRepository repository)
    : EndpointWithoutRequest<IReadOnlyCollection<ProcessExecutionDto>>
{
    public override void Configure()
    {
        Get("/api/process-executions");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var executions = await repository.GetAllAsync(ct);
        var dtos = executions.ToDto();
        await Send.OkAsync(dtos, ct);
    }
}
