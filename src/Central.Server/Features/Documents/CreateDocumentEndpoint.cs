using Central.Domain.Documents.Services;
using Central.Server.Mappers;
using FastEndpoints;
using FluentValidation;

namespace Central.Server.Features.Documents;

public sealed record CreateDocumentRequest
{
    public required string Title { get; init; }
    public DateTimeOffset? DocumentDate { get; init; }
    public string? Content { get; init; }
    public required IFormFile OriginalFile { get; init; }
    public IReadOnlyCollection<long> TagIds { get; init; } = Array.Empty<long>();

    internal sealed class Validator : Validator<CreateDocumentRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
            RuleFor(x => x.OriginalFile).NotNull();
        }
    }
}

public sealed class CreateDocumentEndpoint(
    IDocumentService documentService)
    : Endpoint<CreateDocumentRequest, DocumentDto>
{
    public override void Configure()
    {
        Post("/api/documents");
        AllowFileUploads();
    }

    public override async Task HandleAsync(CreateDocumentRequest req, CancellationToken ct)
    {
        var fileStream = req.OriginalFile.OpenReadStream();
        var fileName = req.OriginalFile.FileName;

        var createdDocument = await documentService.CreateAsync(
            req.Title,
            req.DocumentDate,
            req.Content,
            fileStream,
            fileName,
            req.TagIds,
            ct);

        await fileStream.DisposeAsync();

        await Send.CreatedAtAsync<GetDocumentByIdEndpoint>(
            new { createdDocument.Id },
            createdDocument.ToDto(),
            cancellation: ct);
    }
}
