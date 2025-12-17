using Central.Domain.DocumentTypes.Services;
using Central.Server.Mappers;

using FastEndpoints;

using FluentValidation;

namespace Central.Server.Features.DocumentTypes;

public sealed record UpdateDocumentTypeRequest
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }

    internal sealed class Validator : Validator<UpdateDocumentTypeRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }
}

public sealed class UpdateDocumentTypeEndpoint(IDocumentTypeService documentTypeService)
    : Endpoint<UpdateDocumentTypeRequest, DocumentTypeDto>
{
    public override void Configure()
    {
        Put("/api/document-types/{Id}");
    }

    public override async Task HandleAsync(UpdateDocumentTypeRequest req, CancellationToken ct)
    {
        try
        {
            var documentType = await documentTypeService.UpdateAsync(req.Id, req.Name, req.Description, ct);
            await Send.OkAsync(documentType.ToDto(), ct);
        }
        catch (InvalidOperationException ex)
        {
            AddError(ex.Message);
            await Send.ErrorsAsync(statusCode: 422, cancellation: ct);
        }
    }
}