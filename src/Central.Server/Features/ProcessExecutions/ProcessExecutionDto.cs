namespace Central.Server.Features.ProcessExecutions;

public sealed record ProcessExecutionDto
{
    public required long Id { get; init; }
    public required long ProcessDefinitionId { get; init; }
    public required long DocumentId { get; init; }
    public required string Status { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public string? ErrorMessage { get; init; }
    public IReadOnlyCollection<ProcessExecutionStepDto> Steps { get; init; } = Array.Empty<ProcessExecutionStepDto>();
}

public sealed record ProcessExecutionStepDto
{
    public required long Id { get; init; }
    public required long ProcessExecutionId { get; init; }
    public required string StepName { get; init; }
    public required string StepType { get; init; }
    public required int Order { get; init; }
    public required string Status { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public string? ErrorMessage { get; init; }
    public string? Output { get; init; }
}

public sealed record ExecuteProcessRequest
{
    public required long ProcessDefinitionId { get; init; }
    public required long DocumentId { get; init; }
}