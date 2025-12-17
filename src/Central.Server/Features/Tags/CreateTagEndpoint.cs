using Central.Domain.Tags.Services;
using Central.Server.Mappers;

using FastEndpoints;

using FluentValidation;

namespace Central.Server.Features.Tags;

public sealed record CreateTagRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }

    internal sealed class Validator : Validator<CreateTagRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }
}

public sealed class CreateTagEndpoint(ITagService tagService)
    : Endpoint<CreateTagRequest, TagDto>
{
    public override void Configure()
    {
        Post("/api/tags");
    }

    public override async Task HandleAsync(CreateTagRequest req, CancellationToken ct)
    {
        try
        {
            var tag = await tagService.CreateAsync(req.Name, req.Description, ct);
            await Send.CreatedAtAsync<GetTagByIdEndpoint>(
                new { tag.Id },
                tag.ToDto(),
                cancellation: ct);
        }
        catch (InvalidOperationException ex)
        {
            AddError(ex.Message);
            await Send.ErrorsAsync(statusCode: 422, cancellation: ct);
        }
    }
}