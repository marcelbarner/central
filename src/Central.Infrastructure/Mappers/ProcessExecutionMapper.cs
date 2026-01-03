using Central.Domain.Documents;
using Central.Infrastructure.Entities;

using Riok.Mapperly.Abstractions;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Static mapper for converting between ProcessExecution domain model and ProcessExecutionEntity.
/// </summary>
[Mapper]
public static partial class ProcessExecutionMapper
{
    /// <summary>
    /// Maps a domain ProcessExecution to a ProcessExecutionEntity.
    /// </summary>
    [MapperIgnoreTarget(nameof(ProcessExecutionEntity.ProcessDefinition))]
    [MapperIgnoreTarget(nameof(ProcessExecutionEntity.Document))]
    [MapperIgnoreTarget(nameof(ProcessExecutionEntity.Steps))]
    public static partial ProcessExecutionEntity ToEntity(this ProcessExecution execution);

    /// <summary>
    /// Maps a ProcessExecutionEntity to a domain ProcessExecution.
    /// </summary>
    [MapperIgnoreSource(nameof(ProcessExecutionEntity.ProcessDefinition))]
    [MapperIgnoreSource(nameof(ProcessExecutionEntity.Document))]
    [MapperIgnoreTarget(nameof(ProcessExecution.Steps))]
    private static partial ProcessExecution ToDomainInternal(this ProcessExecutionEntity entity);

    /// <summary>
    /// Maps a ProcessExecutionEntity to a domain ProcessExecution with steps.
    /// </summary>
    public static ProcessExecution ToDomain(this ProcessExecutionEntity entity)
    {
        var execution = entity.ToDomainInternal();

        return execution with
        {
            Steps = entity.Steps
                .OrderBy(s => s.Order)
                .Select(s => s.ToDomain())
                .ToList()
        };
    }

    /// <summary>
    /// Maps a collection of ProcessExecutionEntity to domain ProcessExecutions.
    /// </summary>
    public static IReadOnlyCollection<ProcessExecution> ToDomain(this IEnumerable<ProcessExecutionEntity> entities)
        => entities.Select(e => e.ToDomain()).ToList();

    /// <summary>
    /// Updates a ProcessExecutionEntity from a domain ProcessExecution.
    /// </summary>
    [MapperIgnoreTarget(nameof(ProcessExecutionEntity.ProcessDefinition))]
    [MapperIgnoreTarget(nameof(ProcessExecutionEntity.Document))]
    [MapperIgnoreTarget(nameof(ProcessExecutionEntity.Steps))]
    public static partial void UpdateEntity(this ProcessExecution execution, ProcessExecutionEntity entity);
}