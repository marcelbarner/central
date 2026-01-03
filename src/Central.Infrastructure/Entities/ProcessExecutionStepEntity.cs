using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Central.Domain.Documents;

namespace Central.Infrastructure.Entities;

/// <summary>
/// Entity representing a process execution step result in the database.
/// </summary>
[Table("ProcessExecutionSteps")]
public sealed class ProcessExecutionStepEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the process execution.
    /// </summary>
    public long ProcessExecutionId { get; set; }

    /// <summary>
    /// Gets or sets the name of the step.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string StepName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of step.
    /// </summary>
    public StepType StepType { get; set; }

    /// <summary>
    /// Gets or sets the execution order.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the execution status.
    /// </summary>
    public ExecutionStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the step started.
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the step completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the error message if the step failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the output or result data.
    /// </summary>
    public string? Output { get; set; }

    /// <summary>
    /// Gets or sets the process execution navigation property.
    /// </summary>
    [ForeignKey(nameof(ProcessExecutionId))]
    public ProcessExecutionEntity? ProcessExecution { get; set; }
}