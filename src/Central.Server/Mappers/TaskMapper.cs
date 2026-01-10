using Central.Domain.Documents;
using Central.Server.Features.Tasks;

namespace Central.Server.Mappers;

public static class TaskMapper
{
    public static TaskDto ToDto(this ProcessingTask task) => new()
    {
        Id = task.Id,
        Name = task.Name,
        Description = task.Description,
        TaskType = task.TaskType.ToString(),
        Enabled = task.Enabled,
        Configuration = new TaskConfigurationDto
        {
            AzureEndpoint = task.Configuration.AzureEndpoint,
            AzureApiKey = task.Configuration.AzureApiKey,
            AzureModelOrDeployment = task.Configuration.AzureModelOrDeployment,
            Prompt = task.Configuration.Prompt,
            Temperature = task.Configuration.Temperature,
            MaxTokens = task.Configuration.MaxTokens,
            Capabilities = task.Configuration.Capabilities,
            DocumentIntelligenceOptions = task.Configuration.DocumentIntelligenceOptions
        },
        Created = task.Created,
        Updated = task.Updated
    };

    public static TaskExecutionDto ToDto(this TaskExecution execution) => new()
    {
        Id = execution.Id,
        TaskId = execution.TaskId,
        DocumentId = execution.DocumentId,
        PipelineExecutionId = execution.PipelineExecutionId,
        Status = execution.Status.ToString(),
        StartedAt = execution.StartedAt,
        CompletedAt = execution.CompletedAt,
        ErrorMessage = execution.ErrorMessage,
        Result = execution.Result
    };
}
