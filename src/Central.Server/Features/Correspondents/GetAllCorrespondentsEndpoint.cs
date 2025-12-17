using Central.Domain.Correspondents.Services;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Correspondents;

public sealed class GetAllCorrespondentsEndpoint(ICorrespondentService correspondentService)
    : EndpointWithoutRequest<IReadOnlyCollection<CorrespondentDto>>
{
    public override void Configure()
    {
        Get("/api/correspondents");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var correspondents = await correspondentService.GetAllAsync(ct);
        var dtos = correspondents.Select(c => c.ToDto()).ToList();
        await Send.OkAsync(dtos, ct);
    }
}