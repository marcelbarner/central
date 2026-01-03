using Central.Domain.Documents;
using Central.Infrastructure.Entities;

using Riok.Mapperly.Abstractions;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Static mapper for converting between ProcessDefinition domain model and ProcessDefinitionEntity.
/// </summary>
[Mapper]
public static partial class ProcessDefinitionMapper
{
    /// <summary>
    /// Maps a domain ProcessDefinition to a ProcessDefinitionEntity.
    /// </summary>
    [MapperIgnoreTarget(nameof(ProcessDefinitionEntity.Steps))]
    public static partial ProcessDefinitionEntity ToEntity(this ProcessDefinition processDefinition);

    /// <summary>
    /// Maps a ProcessDefinitionEntity to a domain ProcessDefinition.
    /// </summary>
    [MapperIgnoreTarget(nameof(ProcessDefinition.Steps))]
    private static partial ProcessDefinition ToDomainInternal(this ProcessDefinitionEntity entity);

    /// <summary>
    /// Maps a ProcessDefinitionEntity to a domain ProcessDefinition with steps.
    /// </summary>
    public static ProcessDefinition ToDomain(this ProcessDefinitionEntity entity)
    {
        var processDefinition = entity.ToDomainInternal();

        return processDefinition with
        {
            Steps = entity.Steps
                .OrderBy(s => s.Order)
                .Select(s => s.ToDomain())
                .ToList()
        };
    }

    /// <summary>
    /// Maps a collection of ProcessDefinitionEntity to domain ProcessDefinitions.
    /// </summary>
    public static IReadOnlyCollection<ProcessDefinition> ToDomain(this IEnumerable<ProcessDefinitionEntity> entities)
        => entities.Select(e => e.ToDomain()).ToList();

    /// <summary>
    /// Updates a ProcessDefinitionEntity from a domain ProcessDefinition.
    /// </summary>
    [MapperIgnoreTarget(nameof(ProcessDefinitionEntity.Steps))]
    public static partial void UpdateEntity(this ProcessDefinition processDefinition, ProcessDefinitionEntity entity);
}