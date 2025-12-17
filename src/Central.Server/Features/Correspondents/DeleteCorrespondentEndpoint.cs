using Central.Domain.Correspondents.Services;
using FastEndpoints;

namespace Central.Server.Features.Correspondents;

public sealed record DeleteCorrespondentRequest
{
    public long Id { get; init; }
}

public sealed class DeleteCorrespondentEndpoint(ICorrespondentService correspondentService)
    : Endpoint<DeleteCorrespondentRequest>
{
    public override void Configure()
    {
        Delete("/api/correspondents/{Id}");
    }

    public override async Task HandleAsync(DeleteCorrespondentRequest req, CancellationToken ct)
    {
        try
        {
            await correspondentService.DeleteAsync(req.Id, ct);
            await Send.NoContentAsync(ct);
        }
        catch (InvalidOperationException)
        {
            await Send.NotFoundAsync(ct);
        }
    }
}
