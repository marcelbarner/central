using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.ProcessDefinitions;

public sealed class GetAllProcessDefinitionsEndpoint(IProcessDefinitionRepository repository)
    : EndpointWithoutRequest<IReadOnlyCollection<ProcessDefinitionDto>>
{
    public override void Configure()
    {
        Get("/api/process-definitions");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var processDefinitions = await repository.GetAllAsync(ct);
        var dtos = processDefinitions.ToDto();
        await Send.OkAsync(dtos, ct);
    }
}