using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Pipelines;

public sealed class CreatePipelineEndpoint(IPipelineRepository repository)
    : Endpoint<CreatePipelineRequest, PipelineDto>
{
    public override void Configure()
    {
        Post("/api/pipelines");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreatePipelineRequest req, CancellationToken ct)
    {
        var steps = req.Steps.Select(s => new PipelineStep
        {
            Id = 0,
            PipelineId = 0,
            Name = s.Name,
            StepType = Enum.Parse<PipelineStepType>(s.StepType),
            Order = s.Order,
            TaskId = s.TaskId,
            WaitDurationSeconds = s.WaitDurationSeconds
        }).ToList();

        DocumentState? triggerState = req.TriggerState != null 
            ? Enum.Parse<DocumentState>(req.TriggerState) 
            : null;

        var pipeline = new Pipeline
        {
            Id = 0,
            Name = req.Name,
            Description = req.Description,
            Enabled = req.Enabled,
            TriggerState = triggerState,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            Steps = steps
        };

        var created = await repository.CreateAsync(pipeline, ct);
        var dto = created.ToDto();

        await Send.CreatedAtAsync<GetPipelineByIdEndpoint>(
            new { id = created.Id },
            dto,
            cancellation: ct);
    }
}
