using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Central.Domain.Documents;

namespace Central.Infrastructure.Entities;

/// <summary>
/// Entity representing a pipeline step in the database.
/// </summary>
[Table("PipelineSteps")]
public sealed class PipelineStepEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the pipeline this step belongs to.
    /// </summary>
    public long PipelineId { get; set; }

    /// <summary>
    /// Gets or sets the name of the step.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of pipeline step.
    /// </summary>
    public PipelineStepType StepType { get; set; }

    /// <summary>
    /// Gets or sets the execution order.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the ID of the task (for TaskStep).
    /// </summary>
    public long? TaskId { get; set; }

    /// <summary>
    /// Gets or sets the wait duration in seconds (for WaitStep).
    /// </summary>
    public int? WaitDurationSeconds { get; set; }

    /// <summary>
    /// Gets or sets the parent pipeline.
    /// </summary>
    [ForeignKey(nameof(PipelineId))]
    public PipelineEntity Pipeline { get; set; } = null!;

    /// <summary>
    /// Gets or sets the referenced task (for TaskStep).
    /// </summary>
    [ForeignKey(nameof(TaskId))]
    public TaskEntity? Task { get; set; }
}
