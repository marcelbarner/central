using Central.Domain.Contracts.Services;

using FastEndpoints;

namespace Central.Server.Features.Contracts;

public sealed record DeleteContractRequest
{
    public required long Id { get; init; }
}

public sealed class DeleteContractEndpoint(IContractService contractService)
    : Endpoint<DeleteContractRequest>
{
    public override void Configure()
    {
        Delete("/api/contracts/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteContractRequest req, CancellationToken ct)
    {
        try
        {
            var deleted = await contractService.DeleteAsync(req.Id, ct);
            if (!deleted)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            await Send.NoContentAsync(ct);
        }
        catch (InvalidOperationException ex)
        {
            ThrowError(ex.Message, 400);
        }
    }
}