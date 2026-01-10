using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Tasks;

public sealed class GetTaskExecutionByIdEndpoint(ITaskExecutionRepository repository)
    : EndpointWithoutRequest<TaskExecutionDto>
{
    public override void Configure()
    {
        Get("/api/task-executions/{id}");
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
