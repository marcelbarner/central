using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Central.Domain.Documents;

namespace Central.Infrastructure.Entities;

/// <summary>
/// Entity representing a process execution in the database.
/// </summary>
[Table("ProcessExecutions")]
public sealed class ProcessExecutionEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the process definition.
    /// </summary>
    public long ProcessDefinitionId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the document being processed.
    /// </summary>
    public long DocumentId { get; set; }

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
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the process definition navigation property.
    /// </summary>
    [ForeignKey(nameof(ProcessDefinitionId))]
    public ProcessDefinitionEntity? ProcessDefinition { get; set; }

    /// <summary>
    /// Gets or sets the document navigation property.
    /// </summary>
    [ForeignKey(nameof(DocumentId))]
    public DocumentEntity? Document { get; set; }

    /// <summary>
    /// Gets or sets the collection of step execution results.
    /// </summary>
    public List<ProcessExecutionStepEntity> Steps { get; set; } = new();
}