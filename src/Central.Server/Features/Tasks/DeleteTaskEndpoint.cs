using Central.Domain.Documents.Ports;

using FastEndpoints;

namespace Central.Server.Features.Tasks;

public sealed class DeleteTaskEndpoint(ITaskRepository repository)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/api/tasks/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");
        await repository.DeleteAsync(id, ct);
        await Send.NoContentAsync(ct);
    }
}
