namespace Central.Server.Features.Tasks;

public sealed record TaskDto
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string TaskType { get; init; }
    public required bool Enabled { get; init; }
    public required TaskConfigurationDto Configuration { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required DateTimeOffset Updated { get; init; }
}

public sealed record TaskConfigurationDto
{
    public string? AzureEndpoint { get; init; }
    public string? AzureApiKey { get; init; }
    public string? AzureModelOrDeployment { get; init; }
    public string? Prompt { get; init; }
    public double? Temperature { get; init; }
    public int? MaxTokens { get; init; }
    public string? Capabilities { get; init; }
    public string? DocumentIntelligenceOptions { get; init; }
}

public sealed record CreateTaskRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string TaskType { get; init; }
    public bool Enabled { get; init; } = true;
    public required TaskConfigurationDto Configuration { get; init; }
}

public sealed record UpdateTaskRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string TaskType { get; init; }
    public required bool Enabled { get; init; }
    public required TaskConfigurationDto Configuration { get; init; }
}

public sealed record ExecuteTaskRequest
{
    public required long DocumentId { get; init; }
}

public sealed record TaskExecutionDto
{
    public required long Id { get; init; }
    public required long TaskId { get; init; }
    public required long DocumentId { get; init; }
    public long? PipelineExecutionId { get; init; }
    public required string Status { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public string? ErrorMessage { get; init; }
    public string? Result { get; init; }
}
