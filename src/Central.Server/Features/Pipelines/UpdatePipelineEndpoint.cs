using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Pipelines;

public sealed class UpdatePipelineEndpoint(IPipelineRepository repository)
    : Endpoint<UpdatePipelineRequest, PipelineDto>
{
    public override void Configure()
    {
        Put("/api/pipelines/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdatePipelineRequest req, CancellationToken ct)
    {
        var id = Route<long>("id");
        var existing = await repository.GetByIdAsync(id, ct);

        if (existing == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var steps = req.Steps.Select(s => new PipelineStep
        {
            Id = 0,
            PipelineId = id,
            Name = s.Name,
            StepType = Enum.Parse<PipelineStepType>(s.StepType),
            Order = s.Order,
            TaskId = s.TaskId,
            WaitDurationSeconds = s.WaitDurationSeconds
        }).ToList();

        DocumentState? triggerState = req.TriggerState != null 
            ? Enum.Parse<DocumentState>(req.TriggerState) 
            : null;

        var updated = existing with
        {
            Name = req.Name,
            Description = req.Description,
            Enabled = req.Enabled,
            TriggerState = triggerState,
            Updated = DateTimeOffset.UtcNow,
            Steps = steps
        };

        var result = await repository.UpdateAsync(updated, ct);
        await Send.OkAsync(result.ToDto(), ct);
    }
}
