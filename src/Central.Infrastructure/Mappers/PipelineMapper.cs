using Central.Domain.Documents;
using Central.Infrastructure.Entities;

using Riok.Mapperly.Abstractions;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Static mapper for converting between Pipeline domain model and PipelineEntity.
/// </summary>
[Mapper]
public static partial class PipelineMapper
{
    /// <summary>
    /// Maps a domain Pipeline to a PipelineEntity.
    /// </summary>
    [MapperIgnoreTarget(nameof(PipelineEntity.Steps))]
    [MapperIgnoreSource(nameof(Pipeline.Steps))]
    public static partial PipelineEntity ToEntity(this Pipeline pipeline);

    /// <summary>
    /// Maps a PipelineEntity to a domain Pipeline.
    /// </summary>
    [MapperIgnoreTarget(nameof(Pipeline.Steps))]
    [MapperIgnoreSource(nameof(PipelineEntity.Steps))]
    private static partial Pipeline ToDomainInternal(this PipelineEntity entity);

    /// <summary>
    /// Maps a PipelineEntity to a domain Pipeline with steps.
    /// </summary>
    public static Pipeline ToDomain(this PipelineEntity entity)
    {
        var pipeline = ToDomainInternal(entity);

        return pipeline with
        {
            Steps = entity.Steps
                .Select(s => s.ToDomain())
                .ToList()
        };
    }

    /// <summary>
    /// Updates a PipelineEntity from a domain Pipeline.
    /// </summary>
    public static void UpdateEntity(this PipelineEntity entity, Pipeline pipeline)
    {
        entity.Name = pipeline.Name;
        entity.Description = pipeline.Description;
        entity.Enabled = pipeline.Enabled;
        entity.TriggerState = pipeline.TriggerState;
        entity.Updated = pipeline.Updated;
    }
}
