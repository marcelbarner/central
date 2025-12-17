using Central.Domain.Tags.Services;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Tags;

public sealed record GetTagByIdRequest
{
    public required long Id { get; init; }
}

public sealed class GetTagByIdEndpoint(ITagService tagService)
    : Endpoint<GetTagByIdRequest, TagDto>
{
    public override void Configure()
    {
        Get("/api/tags/{Id}");
    }

    public override async Task HandleAsync(GetTagByIdRequest req, CancellationToken ct)
    {
        var tag = await tagService.GetByIdAsync(req.Id, ct);

        if (tag == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(tag.ToDto(), ct);
    }
}