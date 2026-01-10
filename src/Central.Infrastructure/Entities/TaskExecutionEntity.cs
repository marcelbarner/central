using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Central.Domain.Documents;

namespace Central.Infrastructure.Entities;

/// <summary>
/// Entity representing a task execution in the database.
/// </summary>
[Table("TaskExecutions")]
public sealed class TaskExecutionEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the task being executed.
    /// </summary>
    public long TaskId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the document being processed.
    /// </summary>
    public long DocumentId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the pipeline execution (if executed as part of a pipeline).
    /// </summary>
    public long? PipelineExecutionId { get; set; }

    /// <summary>
    /// Gets or sets the execution status.
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
    /// Gets or sets the JSON result from the AI service.
    /// </summary>
    [Column(TypeName = "TEXT")]
    public string? Result { get; set; }

    /// <summary>
    /// Gets or sets the task that was executed.
    /// </summary>
    [ForeignKey(nameof(TaskId))]
    public TaskEntity Task { get; set; } = null!;

    /// <summary>
    /// Gets or sets the document that was processed.
    /// </summary>
    [ForeignKey(nameof(DocumentId))]
    public DocumentEntity Document { get; set; } = null!;

    /// <summary>
    /// Gets or sets the parent pipeline execution (if applicable).
    /// </summary>
    [ForeignKey(nameof(PipelineExecutionId))]
    public PipelineExecutionEntity? PipelineExecution { get; set; }
}
