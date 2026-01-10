using Central.Domain.Documents;
using Central.Infrastructure.Entities;

using Riok.Mapperly.Abstractions;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Static mapper for converting between Task domain model and TaskEntity.
/// </summary>
[Mapper]
public static partial class TaskMapper
{
    /// <summary>
    /// Maps a domain Task to a TaskEntity.
    /// </summary>
    public static TaskEntity ToEntity(this ProcessingTask task)
    {
        return new TaskEntity
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            TaskType = task.TaskType,
            Enabled = task.Enabled,
            AzureEndpoint = task.Configuration.AzureEndpoint,
            AzureApiKey = task.Configuration.AzureApiKey,
            AzureModelOrDeployment = task.Configuration.AzureModelOrDeployment,
            Prompt = task.Configuration.Prompt,
            Temperature = task.Configuration.Temperature,
            MaxTokens = task.Configuration.MaxTokens,
            Capabilities = task.Configuration.Capabilities,
            DocumentIntelligenceOptions = task.Configuration.DocumentIntelligenceOptions,
            Created = task.Created,
            Updated = task.Updated
        };
    }

    /// <summary>
    /// Maps a TaskEntity to a domain Task.
    /// </summary>
    public static ProcessingTask ToDomain(this TaskEntity entity)
    {
        return new ProcessingTask
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            TaskType = entity.TaskType,
            Enabled = entity.Enabled,
            Configuration = new TaskConfiguration
            {
                AzureEndpoint = entity.AzureEndpoint,
                AzureApiKey = entity.AzureApiKey,
                AzureModelOrDeployment = entity.AzureModelOrDeployment,
                Prompt = entity.Prompt,
                Temperature = entity.Temperature,
                MaxTokens = entity.MaxTokens,
                Capabilities = entity.Capabilities,
                DocumentIntelligenceOptions = entity.DocumentIntelligenceOptions
            },
            Created = entity.Created,
            Updated = entity.Updated
        };
    }

    /// <summary>
    /// Updates a TaskEntity from a domain Task.
    /// </summary>
    public static void UpdateEntity(this TaskEntity entity, ProcessingTask task)
    {
        entity.Name = task.Name;
        entity.Description = task.Description;
        entity.TaskType = task.TaskType;
        entity.Enabled = task.Enabled;
        entity.AzureEndpoint = task.Configuration.AzureEndpoint;
        entity.AzureApiKey = task.Configuration.AzureApiKey;
        entity.AzureModelOrDeployment = task.Configuration.AzureModelOrDeployment;
        entity.Prompt = task.Configuration.Prompt;
        entity.Temperature = task.Configuration.Temperature;
        entity.MaxTokens = task.Configuration.MaxTokens;
        entity.Capabilities = task.Configuration.Capabilities;
        entity.DocumentIntelligenceOptions = task.Configuration.DocumentIntelligenceOptions;
        entity.Updated = task.Updated;
    }
}
