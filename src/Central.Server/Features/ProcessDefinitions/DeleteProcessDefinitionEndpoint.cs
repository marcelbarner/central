using Central.Domain.Documents.Ports;

using FastEndpoints;

namespace Central.Server.Features.ProcessDefinitions;

public sealed class DeleteProcessDefinitionEndpoint(IProcessDefinitionRepository repository)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/api/process-definitions/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");
        var existing = await repository.GetByIdAsync(id, ct);
        if (existing == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await repository.DeleteAsync(id, ct);
        await Send.NoContentAsync(ct);
    }
}