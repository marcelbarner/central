using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Central.Domain.Documents;

namespace Central.Infrastructure.Entities;

/// <summary>
/// Entity representing a processing step in the database.
/// </summary>
[Table("ProcessingSteps")]
public sealed class ProcessingStepEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the process definition this step belongs to.
    /// </summary>
    public long ProcessDefinitionId { get; set; }

    /// <summary>
    /// Gets or sets the name of the step.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the step.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the type of processing step.
    /// </summary>
    public StepType StepType { get; set; }

    /// <summary>
    /// Gets or sets the execution order.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the Azure endpoint URL.
    /// </summary>
    [MaxLength(500)]
    public string? AzureEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the Azure API key.
    /// </summary>
    [MaxLength(500)]
    public string? AzureApiKey { get; set; }

    /// <summary>
    /// Gets or sets the Azure model or deployment name.
    /// </summary>
    [MaxLength(200)]
    public string? AzureModelOrDeployment { get; set; }

    /// <summary>
    /// Gets or sets the AI prompt.
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// Gets or sets additional configuration as JSON.
    /// </summary>
    public string? Configuration { get; set; }

    /// <summary>
    /// Gets or sets the process definition navigation property.
    /// </summary>
    [ForeignKey(nameof(ProcessDefinitionId))]
    public ProcessDefinitionEntity? ProcessDefinition { get; set; }
}