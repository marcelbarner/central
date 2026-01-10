using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Central.Domain.Documents;

namespace Central.Infrastructure.Entities;

/// <summary>
/// Entity representing a pipeline execution in the database.
/// </summary>
[Table("PipelineExecutions")]
public sealed class PipelineExecutionEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the pipeline being executed.
    /// </summary>
    public long PipelineId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the document being processed.
    /// </summary>
    public long DocumentId { get; set; }

    /// <summary>
    /// Gets or sets the overall execution status.
    /// </summary>
    public ExecutionStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when execution started.
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when execution completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the error message if execution failed.
    /// </summary>
    [Column(TypeName = "TEXT")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the pipeline that was executed.
    /// </summary>
    [ForeignKey(nameof(PipelineId))]
    public PipelineEntity Pipeline { get; set; } = null!;

    /// <summary>
    /// Gets or sets the document that was processed.
    /// </summary>
    [ForeignKey(nameof(DocumentId))]
    public DocumentEntity Document { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of task executions from this pipeline.
    /// </summary>
    public List<TaskExecutionEntity> TaskExecutions { get; set; } = new();
}
