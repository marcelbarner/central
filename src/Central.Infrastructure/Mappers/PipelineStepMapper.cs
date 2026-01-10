using Central.Domain.Documents;
using Central.Infrastructure.Entities;

using Riok.Mapperly.Abstractions;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Static mapper for converting between PipelineStep domain model and PipelineStepEntity.
/// </summary>
[Mapper]
public static partial class PipelineStepMapper
{
    /// <summary>
    /// Maps a domain PipelineStep to a PipelineStepEntity.
    /// </summary>
    [MapperIgnoreTarget(nameof(PipelineStepEntity.Pipeline))]
    [MapperIgnoreTarget(nameof(PipelineStepEntity.Task))]
    public static partial PipelineStepEntity ToEntity(this PipelineStep step);

    /// <summary>
    /// Maps a PipelineStepEntity to a domain PipelineStep.
    /// </summary>
    [MapperIgnoreSource(nameof(PipelineStepEntity.Pipeline))]
    [MapperIgnoreSource(nameof(PipelineStepEntity.Task))]
    public static partial PipelineStep ToDomain(this PipelineStepEntity entity);
}
