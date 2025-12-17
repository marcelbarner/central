using Central.Domain.Tags.Services;
using Central.Server.Mappers;
using FastEndpoints;
using FluentValidation;

namespace Central.Server.Features.Tags;

public sealed record UpdateTagRequest
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }

    internal sealed class Validator : Validator<UpdateTagRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }
}

public sealed class UpdateTagEndpoint(ITagService tagService)
    : Endpoint<UpdateTagRequest, TagDto>
{
    public override void Configure()
    {
        Put("/api/tags/{Id}");
    }

    public override async Task HandleAsync(UpdateTagRequest req, CancellationToken ct)
    {
        try
        {
            var tag = await tagService.UpdateAsync(req.Id, req.Name, req.Description, ct);
            await Send.OkAsync(tag.ToDto(), ct);
        }
        catch (InvalidOperationException ex)
        {
            AddError(ex.Message);
            await Send.ErrorsAsync(statusCode: 422, cancellation: ct);
        }
    }
}
