using Central.Domain.Contracts.Services;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Contracts;

public sealed class GetContractByIdEndpoint(IContractService contractService)
    : Endpoint<GetContractByIdRequest, ContractDto>
{
    public override void Configure()
    {
        Get("/api/contracts/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetContractByIdRequest req, CancellationToken ct)
    {
        var contract = await contractService.GetByIdAsync(req.Id, ct);
        if (contract is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(contract.ToDto(), ct);
    }
}

public sealed record GetContractByIdRequest
{
    public required long Id { get; init; }
}