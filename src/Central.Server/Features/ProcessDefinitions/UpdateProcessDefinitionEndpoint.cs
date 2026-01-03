using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.ProcessDefinitions;

public sealed record UpdateProcessDefinitionEndpointRequest
{
    public long Id { get; init; }
    public required UpdateProcessDefinitionRequest Data { get; init; }
}

public sealed class UpdateProcessDefinitionEndpoint(IProcessDefinitionRepository repository)
    : Endpoint<UpdateProcessDefinitionEndpointRequest, ProcessDefinitionDto>
{
    public override void Configure()
    {
        Put("/api/process-definitions/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateProcessDefinitionEndpointRequest req, CancellationToken ct)
    {
        var existing = await repository.GetByIdAsync(req.Id, ct);
        if (existing == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var updated = req.Data.ToDomain(req.Id, existing.Created);
        updated = await repository.UpdateAsync(updated, ct);

        var dto = updated.ToDto();
        await Send.OkAsync(dto, ct);
    }
}