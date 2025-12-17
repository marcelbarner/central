using Central.Domain.Tags.Services;
using Central.Server.Mappers;
using FastEndpoints;

namespace Central.Server.Features.Tags;

public sealed class GetAllTagsEndpoint(ITagService tagService)
    : EndpointWithoutRequest<IReadOnlyCollection<TagDto>>
{
    public override void Configure()
    {
        Get("/api/tags");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var tags = await tagService.GetAllAsync(ct);
        var dtos = tags.ToDto();
        await Send.OkAsync(dtos, ct);
    }
}
