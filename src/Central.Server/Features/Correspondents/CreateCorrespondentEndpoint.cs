using Central.Domain.Correspondents.Services;
using Central.Server.Mappers;
using FastEndpoints;
using FluentValidation;

namespace Central.Server.Features.Correspondents;

public sealed record CreateCorrespondentRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }

    internal sealed class Validator : Validator<CreateCorrespondentRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }
}

public sealed class CreateCorrespondentEndpoint(ICorrespondentService correspondentService)
    : Endpoint<CreateCorrespondentRequest, CorrespondentDto>
{
    public override void Configure()
    {
        Post("/api/correspondents");
    }

    public override async Task HandleAsync(CreateCorrespondentRequest req, CancellationToken ct)
    {
        try
        {
            var correspondent = await correspondentService.CreateAsync(req.Name, req.Description, ct);
            await Send.CreatedAtAsync<GetCorrespondentByIdEndpoint>(
                new { correspondent.Id },
                correspondent.ToDto(),
                cancellation: ct);
        }
        catch (InvalidOperationException ex)
        {
            ThrowError(ex.Message);
        }
    }
}
