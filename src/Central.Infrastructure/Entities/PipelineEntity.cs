using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Central.Domain.Documents;

namespace Central.Infrastructure.Entities;

/// <summary>
/// Entity representing a pipeline in the database.
/// </summary>
[Table("Pipelines")]
public sealed class PipelineEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the pipeline.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the pipeline.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this pipeline is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the document state that triggers this pipeline.
    /// When null, the pipeline can only be executed manually.
    /// </summary>
    public DocumentState? TriggerState { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the pipeline was created.
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the pipeline was last updated.
    /// </summary>
    public DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Gets or sets the collection of pipeline steps.
    /// </summary>
    public List<PipelineStepEntity> Steps { get; set; } = new();
}
