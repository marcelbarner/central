using Central.Domain.DocumentTypes.Services;
using Central.Server.Mappers;

using FastEndpoints;

using FluentValidation;

namespace Central.Server.Features.DocumentTypes;

public sealed record CreateDocumentTypeRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }

    internal sealed class Validator : Validator<CreateDocumentTypeRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }
}

public sealed class CreateDocumentTypeEndpoint(IDocumentTypeService documentTypeService)
    : Endpoint<CreateDocumentTypeRequest, DocumentTypeDto>
{
    public override void Configure()
    {
        Post("/api/document-types");
    }

    public override async Task HandleAsync(CreateDocumentTypeRequest req, CancellationToken ct)
    {
        try
        {
            var documentType = await documentTypeService.CreateAsync(req.Name, req.Description, ct);
            await Send.CreatedAtAsync<GetDocumentTypeByIdEndpoint>(
                new { documentType.Id },
                documentType.ToDto(),
                cancellation: ct);
        }
        catch (InvalidOperationException ex)
        {
            AddError(ex.Message);
            await Send.ErrorsAsync(statusCode: 422, cancellation: ct);
        }
    }
}