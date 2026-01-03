using Central.Domain.Documents;
using Central.Server.Features.ProcessDefinitions;

using Riok.Mapperly.Abstractions;

namespace Central.Server.Mappers;

/// <summary>
/// Static mapper for converting between ProcessDefinition domain models and DTOs.
/// </summary>
[Mapper]
public static partial class ProcessDefinitionDtoMapper
{
    /// <summary>
    /// Maps a domain ProcessDefinition to ProcessDefinitionDto.
    /// </summary>
    public static partial ProcessDefinitionDto ToDto(this ProcessDefinition processDefinition);

    /// <summary>
    /// Maps a collection of ProcessDefinitions to DTOs.
    /// </summary>
    public static partial IReadOnlyCollection<ProcessDefinitionDto> ToDto(this IEnumerable<ProcessDefinition> processDefinitions);

    /// <summary>
    /// Maps ProcessingStep to ProcessingStepDto.
    /// </summary>
    public static partial ProcessingStepDto ToDto(this ProcessingStep step);

    /// <summary>
    /// Maps CreateProcessDefinitionRequest to domain ProcessDefinition.
    /// </summary>
    public static ProcessDefinition ToDomain(this CreateProcessDefinitionRequest request)
    {
        return new ProcessDefinition
        {
            Id = 0,
            Name = request.Name,
            Description = request.Description,
            Enabled = request.Enabled,
            TriggerState = Enum.Parse<DocumentState>(request.TriggerState),
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            Steps = request.Steps.Select(s => s.ToDomain()).ToList()
        };
    }

    /// <summary>
    /// Maps CreateProcessingStepRequest to domain ProcessingStep.
    /// </summary>
    public static ProcessingStep ToDomain(this CreateProcessingStepRequest request)
    {
        return new ProcessingStep
        {
            Id = 0,
            ProcessDefinitionId = 0,
            Name = request.Name,
            Description = request.Description,
            StepType = Enum.Parse<StepType>(request.StepType),
            Order = request.Order,
            AzureEndpoint = request.AzureEndpoint,
            AzureApiKey = request.AzureApiKey,
            AzureModelOrDeployment = request.AzureModelOrDeployment,
            Prompt = request.Prompt,
            Configuration = request.Configuration
        };
    }

    /// <summary>
    /// Maps UpdateProcessDefinitionRequest to domain ProcessDefinition with existing ID.
    /// </summary>
    public static ProcessDefinition ToDomain(this UpdateProcessDefinitionRequest request, long id, DateTimeOffset created)
    {
        return new ProcessDefinition
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Enabled = request.Enabled,
            TriggerState = Enum.Parse<DocumentState>(request.TriggerState),
            Created = created,
            Updated = DateTimeOffset.UtcNow,
            Steps = request.Steps.Select(s => s.ToDomain()).ToList()
        };
    }

    /// <summary>
    /// Maps UpdateProcessingStepRequest to domain ProcessingStep.
    /// </summary>
    public static ProcessingStep ToDomain(this UpdateProcessingStepRequest request)
    {
        return new ProcessingStep
        {
            Id = request.Id,
            ProcessDefinitionId = 0, // Will be set by repository
            Name = request.Name,
            Description = request.Description,
            StepType = Enum.Parse<StepType>(request.StepType),
            Order = request.Order,
            AzureEndpoint = request.AzureEndpoint,
            AzureApiKey = request.AzureApiKey,
            AzureModelOrDeployment = request.AzureModelOrDeployment,
            Prompt = request.Prompt,
            Configuration = request.Configuration
        };
    }
}