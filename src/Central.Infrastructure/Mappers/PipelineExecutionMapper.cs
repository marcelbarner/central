using Central.Domain.Documents;
using Central.Infrastructure.Entities;

using Riok.Mapperly.Abstractions;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Static mapper for converting between PipelineExecution domain model and PipelineExecutionEntity.
/// </summary>
[Mapper]
public static partial class PipelineExecutionMapper
{
    /// <summary>
    /// Maps a domain PipelineExecution to a PipelineExecutionEntity.
    /// </summary>
    [MapperIgnoreTarget(nameof(PipelineExecutionEntity.Pipeline))]
    [MapperIgnoreTarget(nameof(PipelineExecutionEntity.Document))]
    [MapperIgnoreTarget(nameof(PipelineExecutionEntity.TaskExecutions))]
    [MapperIgnoreSource(nameof(PipelineExecution.TaskExecutions))]
    public static partial PipelineExecutionEntity ToEntity(this PipelineExecution execution);

    /// <summary>
    /// Maps a PipelineExecutionEntity to a domain PipelineExecution.
    /// </summary>
    [MapperIgnoreTarget(nameof(PipelineExecution.TaskExecutions))]
    [MapperIgnoreSource(nameof(PipelineExecutionEntity.Pipeline))]
    [MapperIgnoreSource(nameof(PipelineExecutionEntity.Document))]
    [MapperIgnoreSource(nameof(PipelineExecutionEntity.TaskExecutions))]
    private static partial PipelineExecution ToDomainInternal(this PipelineExecutionEntity entity);

    /// <summary>
    /// Maps a PipelineExecutionEntity to a domain PipelineExecution with task executions.
    /// </summary>
    public static PipelineExecution ToDomain(this PipelineExecutionEntity entity)
    {
        var execution = ToDomainInternal(entity);

        return execution with
        {
            TaskExecutions = entity.TaskExecutions
                .Select(te => te.ToDomain())
                .ToList()
        };
    }
}
