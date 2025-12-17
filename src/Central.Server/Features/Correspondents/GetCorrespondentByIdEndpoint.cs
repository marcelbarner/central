using Central.Domain.Correspondents.Services;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Correspondents;

public sealed record GetCorrespondentByIdRequest
{
    public long Id { get; init; }
}

public sealed class GetCorrespondentByIdEndpoint(ICorrespondentService correspondentService)
    : Endpoint<GetCorrespondentByIdRequest, CorrespondentDto>
{
    public override void Configure()
    {
        Get("/api/correspondents/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetCorrespondentByIdRequest req, CancellationToken ct)
    {
        var correspondent = await correspondentService.GetByIdAsync(req.Id, ct);
        if (correspondent == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(correspondent.ToDto(), ct);
    }
}