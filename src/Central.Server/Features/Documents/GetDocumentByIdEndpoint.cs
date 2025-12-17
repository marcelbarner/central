using Central.Domain.Documents.Services;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Documents;

public sealed record GetDocumentByIdRequest
{
    public required long Id { get; init; }
}

public sealed class GetDocumentByIdEndpoint(IDocumentService documentService)
    : Endpoint<GetDocumentByIdRequest, DocumentDto>
{
    public override void Configure()
    {
        Get("/api/documents/{Id}");
    }

    public override async Task HandleAsync(GetDocumentByIdRequest req, CancellationToken ct)
    {
        var document = await documentService.GetByIdAsync(req.Id, ct);

        if (document == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(document.ToDto(), ct);
    }
}