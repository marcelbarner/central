using Central.Domain.Documents;
using Central.Infrastructure.Entities;

using Riok.Mapperly.Abstractions;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Static mapper for converting between ProcessingStep domain model and ProcessingStepEntity.
/// </summary>
[Mapper]
public static partial class ProcessingStepMapper
{
    /// <summary>
    /// Maps a domain ProcessingStep to a ProcessingStepEntity.
    /// </summary>
    [MapperIgnoreTarget(nameof(ProcessingStepEntity.ProcessDefinition))]
    public static partial ProcessingStepEntity ToEntity(this ProcessingStep step);

    /// <summary>
    /// Maps a ProcessingStepEntity to a domain ProcessingStep.
    /// </summary>
    [MapperIgnoreSource(nameof(ProcessingStepEntity.ProcessDefinition))]
    public static partial ProcessingStep ToDomain(this ProcessingStepEntity entity);

    /// <summary>
    /// Maps a collection of ProcessingStepEntity to domain ProcessingSteps.
    /// </summary>
    public static IReadOnlyCollection<ProcessingStep> ToDomain(this IEnumerable<ProcessingStepEntity> entities)
        => entities.Select(e => e.ToDomain()).ToList();

    /// <summary>
    /// Updates a ProcessingStepEntity from a domain ProcessingStep.
    /// </summary>
    [MapperIgnoreTarget(nameof(ProcessingStepEntity.ProcessDefinition))]
    public static partial void UpdateEntity(this ProcessingStep step, ProcessingStepEntity entity);
}