using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Tasks;

public sealed class GetAllTasksEndpoint(ITaskRepository repository)
    : EndpointWithoutRequest<IReadOnlyCollection<TaskDto>>
{
    public override void Configure()
    {
        Get("/api/tasks");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var tasks = await repository.GetAllAsync(ct);
        var dtos = tasks.Select(t => t.ToDto()).ToList();

        await Send.OkAsync(dtos, ct);
    }
}
