using Central.Domain.Contracts.Ports;
using Central.Domain.Documents.Services;

using FastEndpoints;

using FluentValidation;

namespace Central.Server.Features.Contracts;

public sealed record AssignContractToDocumentRequest
{
    public required long ContractId { get; init; }
    public required long DocumentId { get; init; }
    public bool SyncCorrespondent { get; init; }

    internal sealed class Validator : Validator<AssignContractToDocumentRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ContractId).GreaterThan(0);
            RuleFor(x => x.DocumentId).GreaterThan(0);
        }
    }
}

public sealed class AssignContractToDocumentEndpoint(
    IDocumentService documentService,
    IContractRepository contractRepository)
    : Endpoint<AssignContractToDocumentRequest>
{
    public override void Configure()
    {
        Post("/api/contracts/{contractId}/assign-to-document");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AssignContractToDocumentRequest req, CancellationToken ct)
    {
        try
        {
            // Check if contract exists
            var contract = await contractRepository.GetByIdAsync(req.ContractId, ct);
            if (contract is null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            // Assign contract to document
            var document = await documentService.AssignContractAsync(
                req.DocumentId,
                req.ContractId,
                req.SyncCorrespondent,
                ct);

            // If syncCorrespondent is true and contract has a correspondent, update the document
            if (req.SyncCorrespondent && contract.CorrespondentId.HasValue)
            {
                // Update the document with the contract's correspondent
                await documentService.UpdateAsync(
                    document.Id,
                    document.Title,
                    document.DocumentDate,
                    document.Content,
                    document.DocumentTypeId,
                    contract.CorrespondentId,
                    document.ContractId,
                    document.TagIds,
                    ct);
            }

            await Send.OkAsync(ct);
        }
        catch (InvalidOperationException ex)
        {
            ThrowError(ex.Message, 400);
        }
    }
}