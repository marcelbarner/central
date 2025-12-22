using Central.Domain.Documents.Services;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Documents;

public sealed class GetAllDocumentsEndpoint(IDocumentService documentService)
    : EndpointWithoutRequest<IReadOnlyCollection<DocumentDto>>
{
    public override void Configure()
    {
        Get("/api/documents");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var documents = await documentService.GetAllAsync(ct);
        var dtos = documents.ToDto();
        await Send.OkAsync(dtos, ct);
    }
}