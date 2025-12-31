using Central.Domain.Contracts.Services;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Contracts;

public sealed class GetAllContractsEndpoint(IContractService contractService)
    : EndpointWithoutRequest<IReadOnlyCollection<ContractDto>>
{
    public override void Configure()
    {
        Get("/api/contracts");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var contracts = await contractService.GetAllAsync(ct);
        var dtos = contracts.Select(c => c.ToDto()).ToList();
        await Send.OkAsync(dtos, ct);
    }
}