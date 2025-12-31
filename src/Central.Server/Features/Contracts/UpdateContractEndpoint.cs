using Central.Domain.Contracts;
using Central.Domain.Contracts.Services;
using Central.Server.Mappers;

using FastEndpoints;

using FluentValidation;

namespace Central.Server.Features.Contracts;

public sealed record UpdateContractRequest
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string State { get; init; }
    public long? CorrespondentId { get; init; }
    public string? CustomerId { get; init; }
    public string? ContractId { get; init; }

    internal sealed class Validator : Validator<UpdateContractRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(1000);
            RuleFor(x => x.State).NotEmpty().Must(s => Enum.TryParse<ContractState>(s, out _))
                .WithMessage("State must be Draft, Active, Expired, or Terminated");
            RuleFor(x => x.CustomerId).MaximumLength(100);
            RuleFor(x => x.ContractId).MaximumLength(100);
        }
    }
}

public sealed class UpdateContractEndpoint(IContractService contractService)
    : Endpoint<UpdateContractRequest, ContractDto>
{
    public override void Configure()
    {
        Put("/api/contracts/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateContractRequest req, CancellationToken ct)
    {
        try
        {
            var state = Enum.Parse<ContractState>(req.State);
            var contract = await contractService.UpdateAsync(
                req.Id,
                req.Name,
                req.Description,
                state,
                req.CorrespondentId,
                req.CustomerId,
                req.ContractId,
                ct);

            await Send.OkAsync(contract.ToDto(), ct);
        }
        catch (InvalidOperationException ex)
        {
            ThrowError(ex.Message);
        }
    }
}