using Central.Domain.Documents;
using Central.Server.Features.Pipelines;

namespace Central.Server.Mappers;

public static class PipelineMapper
{
    public static PipelineDto ToDto(this Pipeline pipeline) => new()
    {
        Id = pipeline.Id,
        Name = pipeline.Name,
        Description = pipeline.Description,
        Enabled = pipeline.Enabled,
        TriggerState = pipeline.TriggerState?.ToString(),
        Created = pipeline.Created,
        Updated = pipeline.Updated,
        Steps = pipeline.Steps.Select(s => s.ToDto()).ToList()
    };

    public static PipelineStepDto ToDto(this PipelineStep step) => new()
    {
        Id = step.Id,
        Name = step.Name,
        StepType = step.StepType.ToString(),
        Order = step.Order,
        TaskId = step.TaskId,
        WaitDurationSeconds = step.WaitDurationSeconds
    };

    public static PipelineExecutionDto ToDto(this PipelineExecution execution) => new()
    {
        Id = execution.Id,
        PipelineId = execution.PipelineId,
        DocumentId = execution.DocumentId,
        Status = execution.Status.ToString(),
        StartedAt = execution.StartedAt,
        CompletedAt = execution.CompletedAt,
        ErrorMessage = execution.ErrorMessage,
        TaskExecutionIds = execution.TaskExecutions?.Select(te => te.Id).ToList() ?? []
    };
}
