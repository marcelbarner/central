namespace Central.Server.Features.ProcessDefinitions;

public sealed record ProcessDefinitionDto
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required bool Enabled { get; init; }
    public required string TriggerState { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required DateTimeOffset Updated { get; init; }
    public IReadOnlyCollection<ProcessingStepDto> Steps { get; init; } = Array.Empty<ProcessingStepDto>();
}

public sealed record ProcessingStepDto
{
    public required long Id { get; init; }
    public required long ProcessDefinitionId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string StepType { get; init; }
    public required int Order { get; init; }
    public string? AzureEndpoint { get; init; }
    public string? AzureApiKey { get; init; }
    public string? AzureModelOrDeployment { get; init; }
    public string? Prompt { get; init; }
    public string? Configuration { get; init; }
}

public sealed record CreateProcessDefinitionRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public bool Enabled { get; init; }
    public required string TriggerState { get; init; }
    public IReadOnlyCollection<CreateProcessingStepRequest> Steps { get; init; } = Array.Empty<CreateProcessingStepRequest>();
}

public sealed record CreateProcessingStepRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string StepType { get; init; }
    public required int Order { get; init; }
    public string? AzureEndpoint { get; init; }
    public string? AzureApiKey { get; init; }
    public string? AzureModelOrDeployment { get; init; }
    public string? Prompt { get; init; }
    public string? Configuration { get; init; }
}

public sealed record UpdateProcessDefinitionRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public bool Enabled { get; init; }
    public required string TriggerState { get; init; }
    public IReadOnlyCollection<UpdateProcessingStepRequest> Steps { get; init; } = Array.Empty<UpdateProcessingStepRequest>();
}

public sealed record UpdateProcessingStepRequest
{
    public long Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string StepType { get; init; }
    public required int Order { get; init; }
    public string? AzureEndpoint { get; init; }
    public string? AzureApiKey { get; init; }
    public string? AzureModelOrDeployment { get; init; }
    public string? Prompt { get; init; }
    public string? Configuration { get; init; }
}