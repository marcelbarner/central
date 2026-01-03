using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.ProcessExecutions;

public sealed class GetProcessExecutionsByDocumentEndpoint(IProcessExecutionRepository repository)
    : EndpointWithoutRequest<IReadOnlyCollection<ProcessExecutionDto>>
{
    public override void Configure()
    {
        Get("/api/documents/{documentId}/process-executions");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var documentId = Route<long>("documentId");
        var executions = await repository.GetByDocumentIdAsync(documentId, ct);
        var dtos = executions.ToDto();
        await Send.OkAsync(dtos, ct);
    }
}