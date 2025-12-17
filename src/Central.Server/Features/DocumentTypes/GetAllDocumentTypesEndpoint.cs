using Central.Domain.DocumentTypes.Services;
using Central.Server.Mappers;
using FastEndpoints;

namespace Central.Server.Features.DocumentTypes;

public sealed class GetAllDocumentTypesEndpoint(IDocumentTypeService documentTypeService)
    : EndpointWithoutRequest<IReadOnlyCollection<DocumentTypeDto>>
{
    public override void Configure()
    {
        Get("/api/document-types");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var documentTypes = await documentTypeService.GetAllAsync(ct);
        var dtos = documentTypes.ToDto();
        await Send.OkAsync(dtos, ct);
    }
}
