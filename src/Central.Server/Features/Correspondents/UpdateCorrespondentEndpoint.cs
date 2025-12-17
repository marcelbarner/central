using Central.Domain.Correspondents.Services;
using Central.Server.Mappers;
using FastEndpoints;
using FluentValidation;

namespace Central.Server.Features.Correspondents;

public sealed record UpdateCorrespondentRequest
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }

    internal sealed class Validator : Validator<UpdateCorrespondentRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }
}

public sealed class UpdateCorrespondentEndpoint(ICorrespondentService correspondentService)
    : Endpoint<UpdateCorrespondentRequest, CorrespondentDto>
{
    public override void Configure()
    {
        Put("/api/correspondents/{Id}");
    }

    public override async Task HandleAsync(UpdateCorrespondentRequest req, CancellationToken ct)
    {
        try
        {
            var correspondent = await correspondentService.UpdateAsync(req.Id, req.Name, req.Description, ct);
            await Send.OkAsync(correspondent.ToDto(), ct);
        }
        catch (InvalidOperationException)
        {
            await Send.NotFoundAsync(ct);
        }
    }
}
