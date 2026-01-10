using Central.Domain.Documents;
using Central.Infrastructure.Entities;

using Riok.Mapperly.Abstractions;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Static mapper for converting between TaskExecution domain model and TaskExecutionEntity.
/// </summary>
[Mapper]
public static partial class TaskExecutionMapper
{
    /// <summary>
    /// Maps a domain TaskExecution to a TaskExecutionEntity.
    /// </summary>
    [MapperIgnoreTarget(nameof(TaskExecutionEntity.Task))]
    [MapperIgnoreTarget(nameof(TaskExecutionEntity.Document))]
    [MapperIgnoreTarget(nameof(TaskExecutionEntity.PipelineExecution))]
    public static partial TaskExecutionEntity ToEntity(this TaskExecution execution);

    /// <summary>
    /// Maps a TaskExecutionEntity to a domain TaskExecution.
    /// </summary>
    [MapperIgnoreSource(nameof(TaskExecutionEntity.Task))]
    [MapperIgnoreSource(nameof(TaskExecutionEntity.Document))]
    [MapperIgnoreSource(nameof(TaskExecutionEntity.PipelineExecution))]
    public static partial TaskExecution ToDomain(this TaskExecutionEntity entity);
}
