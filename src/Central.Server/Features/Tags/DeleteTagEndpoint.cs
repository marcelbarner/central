using Central.Domain.Tags.Services;

using FastEndpoints;

namespace Central.Server.Features.Tags;

public sealed record DeleteTagRequest
{
    public required long Id { get; init; }
}

public sealed class DeleteTagEndpoint(ITagService tagService)
    : Endpoint<DeleteTagRequest>
{
    public override void Configure()
    {
        Delete("/api/tags/{Id}");
    }

    public override async Task HandleAsync(DeleteTagRequest req, CancellationToken ct)
    {
        await tagService.DeleteAsync(req.Id, ct);
        await Send.NoContentAsync(ct);
    }
}