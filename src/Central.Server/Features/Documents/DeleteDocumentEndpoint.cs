using Central.Domain.Documents.Services;
using FastEndpoints;

namespace Central.Server.Features.Documents;

public sealed record DeleteDocumentRequest
{
    public required long Id { get; init; }
}

public sealed class DeleteDocumentEndpoint(IDocumentService documentService)
    : Endpoint<DeleteDocumentRequest>
{
    public override void Configure()
    {
        Delete("/api/documents/{Id}");
    }

    public override async Task HandleAsync(DeleteDocumentRequest req, CancellationToken ct)
    {
        await documentService.DeleteAsync(req.Id, ct);
        await Send.NoContentAsync(ct);
    }
}
