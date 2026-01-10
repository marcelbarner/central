using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Tasks;

public sealed class GetTaskByIdEndpoint(ITaskRepository repository)
    : EndpointWithoutRequest<TaskDto>
{
    public override void Configure()
    {
        Get("/api/tasks/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");
        var task = await repository.GetByIdAsync(id, ct);

        if (task == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(task.ToDto(), ct);
    }
}
