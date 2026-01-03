using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.ProcessDefinitions;

public sealed class GetProcessDefinitionByIdEndpoint(IProcessDefinitionRepository repository)
    : EndpointWithoutRequest<ProcessDefinitionDto>
{
    public override void Configure()
    {
        Get("/api/process-definitions/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");
        var processDefinition = await repository.GetByIdAsync(id, ct);

        if (processDefinition == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var dto = processDefinition.ToDto();
        await Send.OkAsync(dto, ct);
    }
}