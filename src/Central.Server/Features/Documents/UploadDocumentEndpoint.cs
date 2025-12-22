using Central.Domain.Documents.Services;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Documents;

public sealed record UploadDocumentRequest
{
    public required IFormFile File { get; init; }
}

public sealed class UploadDocumentEndpoint(
    IDocumentService documentService)
    : Endpoint<UploadDocumentRequest, DocumentDto>
{
    public override void Configure()
    {
        Post("/api/documents/upload");
        AllowFileUploads();
        AllowAnonymous();
    }

    public override async Task HandleAsync(UploadDocumentRequest req, CancellationToken ct)
    {
        await using var stream = req.File.OpenReadStream();
        var createdDocument = await documentService.CreateFromFileAsync(
            stream,
            req.File.FileName,
            ct);

        await Send.CreatedAtAsync<GetDocumentByIdEndpoint>(
            new { createdDocument.Id },
            createdDocument.ToDto(),
            cancellation: ct);
    }
}