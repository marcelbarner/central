namespace Central.Server.Features.Pipelines;

public sealed record PipelineDto
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required bool Enabled { get; init; }
    public string? TriggerState { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required DateTimeOffset Updated { get; init; }
    public required IReadOnlyCollection<PipelineStepDto> Steps { get; init; }
}

public sealed record PipelineStepDto
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string StepType { get; init; }
    public required int Order { get; init; }
    public long? TaskId { get; init; }
    public int? WaitDurationSeconds { get; init; }
}

public sealed record CreatePipelineRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public bool Enabled { get; init; } = true;
    public string? TriggerState { get; init; }
    public required IReadOnlyCollection<CreatePipelineStepRequest> Steps { get; init; }
}

public sealed record CreatePipelineStepRequest
{
    public required string Name { get; init; }
    public required string StepType { get; init; }
    public required int Order { get; init; }
    public long? TaskId { get; init; }
    public int? WaitDurationSeconds { get; init; }
}

public sealed record UpdatePipelineRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required bool Enabled { get; init; }
    public string? TriggerState { get; init; }
    public required IReadOnlyCollection<UpdatePipelineStepRequest> Steps { get; init; }
}

public sealed record UpdatePipelineStepRequest
{
    public required string Name { get; init; }
    public required string StepType { get; init; }
    public required int Order { get; init; }
    public long? TaskId { get; init; }
    public int? WaitDurationSeconds { get; init; }
}

public sealed record ExecutePipelineRequest
{
    public required long DocumentId { get; init; }
}

public sealed record PipelineExecutionDto
{
    public required long Id { get; init; }
    public required long PipelineId { get; init; }
    public required long DocumentId { get; init; }
    public required string Status { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public string? ErrorMessage { get; init; }
    public required IReadOnlyCollection<long> TaskExecutionIds { get; init; }
}
