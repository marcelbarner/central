using Central.Domain.Documents.Services;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Tasks;

public sealed class ExecuteTaskEndpoint(TaskExecutionService taskExecutionService)
    : Endpoint<ExecuteTaskRequest, TaskExecutionDto>
{
    public override void Configure()
    {
        Post("/api/tasks/{id}/execute");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ExecuteTaskRequest req, CancellationToken ct)
    {
        var taskId = Route<long>("id");
        var execution = await taskExecutionService.ExecuteTaskAsync(taskId, req.DocumentId, null, ct);

        await Send.CreatedAtAsync<GetTaskExecutionByIdEndpoint>(
            new { id = execution.Id },
            execution.ToDto(),
            cancellation: ct);
    }
}
