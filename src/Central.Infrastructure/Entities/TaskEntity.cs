using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Central.Domain.Documents;

namespace Central.Infrastructure.Entities;

/// <summary>
/// Entity representing a task in the database.
/// </summary>
[Table("Tasks")]
public sealed class TaskEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the task.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the type of task.
    /// </summary>
    public TaskType TaskType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this task is enabled.
    /// </summary>
    public bool Enabled { get; set; }

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
    [Column(TypeName = "TEXT")]
    public string? Prompt { get; set; }

    /// <summary>
    /// Gets or sets the sampling temperature for OpenAI tasks.
    /// </summary>
    public double? Temperature { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of tokens.
    /// </summary>
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Gets or sets enabled capabilities (JSON).
    /// </summary>
    [Column(TypeName = "TEXT")]
    public string? Capabilities { get; set; }

    /// <summary>
    /// Gets or sets Document Intelligence options (JSON).
    /// </summary>
    [Column(TypeName = "TEXT")]
    public string? DocumentIntelligenceOptions { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the task was created.
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the task was last updated.
    /// </summary>
    public DateTimeOffset Updated { get; set; }
}
