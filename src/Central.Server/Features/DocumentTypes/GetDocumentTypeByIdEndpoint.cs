using Central.Domain.DocumentTypes.Services;
using Central.Server.Mappers;
using FastEndpoints;

namespace Central.Server.Features.DocumentTypes;

public sealed record GetDocumentTypeByIdRequest
{
    public required long Id { get; init; }
}

public sealed class GetDocumentTypeByIdEndpoint(IDocumentTypeService documentTypeService)
    : Endpoint<GetDocumentTypeByIdRequest, DocumentTypeDto>
{
    public override void Configure()
    {
        Get("/api/document-types/{Id}");
    }

    public override async Task HandleAsync(GetDocumentTypeByIdRequest req, CancellationToken ct)
    {
        var documentType = await documentTypeService.GetByIdAsync(req.Id, ct);

        if (documentType == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(documentType.ToDto(), ct);
    }
}
