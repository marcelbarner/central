using Central.Domain.DocumentTypes.Services;
using FastEndpoints;

namespace Central.Server.Features.DocumentTypes;

public sealed record DeleteDocumentTypeRequest
{
    public required long Id { get; init; }
}

public sealed class DeleteDocumentTypeEndpoint(IDocumentTypeService documentTypeService)
    : Endpoint<DeleteDocumentTypeRequest>
{
    public override void Configure()
    {
        Delete("/api/document-types/{Id}");
    }

    public override async Task HandleAsync(DeleteDocumentTypeRequest req, CancellationToken ct)
    {
        await documentTypeService.DeleteAsync(req.Id, ct);
        await Send.NoContentAsync(ct);
    }
}
