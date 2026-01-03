using Central.Domain.Documents.Services;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.ProcessExecutions;

public sealed class ExecuteProcessEndpoint(IProcessExecutionService executionService)
    : Endpoint<ExecuteProcessRequest, ProcessExecutionDto>
{
    public override void Configure()
    {
        Post("/api/process-executions");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ExecuteProcessRequest req, CancellationToken ct)
    {
        var execution = await executionService.ExecuteProcessAsync(
            req.ProcessDefinitionId,
            req.DocumentId,
            ct);

        var dto = execution.ToDto();
        await Send.CreatedAtAsync<GetProcessExecutionByIdEndpoint>(
            new { id = execution.Id },
            dto,
            cancellation: ct);
    }
}