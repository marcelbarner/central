using Central.Domain.Documents.Services;
using Central.Server.Mappers;
using FastEndpoints;
using FluentValidation;

namespace Central.Server.Features.Documents;

public sealed record UpdateDocumentRequest
{
    public required long Id { get; init; }
    public required string Title { get; init; }
    public DateTimeOffset? DocumentDate { get; init; }
    public string? Content { get; init; }
    public long? DocumentTypeId { get; init; }
    public long? CorrespondentId { get; init; }
    public IReadOnlyCollection<long> TagIds { get; init; } = Array.Empty<long>();

    internal sealed class Validator : Validator<UpdateDocumentRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        }
    }
}

public sealed class UpdateDocumentEndpoint(
    IDocumentService documentService)
    : Endpoint<UpdateDocumentRequest, DocumentDto>
{
    public override void Configure()
    {
        Put("/api/documents/{Id}");
    }

    public override async Task HandleAsync(UpdateDocumentRequest req, CancellationToken ct)
    {
        try
        {
            var result = await documentService.UpdateAsync(
                req.Id,
                req.Title,
                req.DocumentDate,
                req.Content,
                req.DocumentTypeId,
                req.CorrespondentId,
                req.TagIds,
                ct);

            await Send.OkAsync(result.ToDto(), ct);
        }
        catch (InvalidOperationException)
        {
            await Send.NotFoundAsync(ct);
        }
    }
}
