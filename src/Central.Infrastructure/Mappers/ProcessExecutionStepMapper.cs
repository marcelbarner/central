using Central.Domain.Documents;
using Central.Infrastructure.Entities;

using Riok.Mapperly.Abstractions;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Static mapper for converting between ProcessExecutionStep domain model and ProcessExecutionStepEntity.
/// </summary>
[Mapper]
public static partial class ProcessExecutionStepMapper
{
    /// <summary>
    /// Maps a domain ProcessExecutionStep to a ProcessExecutionStepEntity.
    /// </summary>
    [MapperIgnoreTarget(nameof(ProcessExecutionStepEntity.ProcessExecution))]
    public static partial ProcessExecutionStepEntity ToEntity(this ProcessExecutionStep step);

    /// <summary>
    /// Maps a ProcessExecutionStepEntity to a domain ProcessExecutionStep.
    /// </summary>
    [MapperIgnoreSource(nameof(ProcessExecutionStepEntity.ProcessExecution))]
    public static partial ProcessExecutionStep ToDomain(this ProcessExecutionStepEntity entity);

    /// <summary>
    /// Maps a collection of ProcessExecutionStepEntity to domain ProcessExecutionSteps.
    /// </summary>
    public static IReadOnlyCollection<ProcessExecutionStep> ToDomain(this IEnumerable<ProcessExecutionStepEntity> entities)
        => entities.Select(e => e.ToDomain()).ToList();

    /// <summary>
    /// Updates a ProcessExecutionStepEntity from a domain ProcessExecutionStep.
    /// </summary>
    [MapperIgnoreTarget(nameof(ProcessExecutionStepEntity.ProcessExecution))]
    public static partial void UpdateEntity(this ProcessExecutionStep step, ProcessExecutionStepEntity entity);
}