using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Central.Infrastructure.Entities;

/// <summary>
/// Entity representing a document type in the database.
/// </summary>
[Table("DocumentTypes")]
public sealed class DocumentTypeEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the document type.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional description.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the document type was created.
    /// </summary>
    [Required]
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the document type was last updated.
    /// </summary>
    [Required]
    public DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Navigation property for documents that have this document type.
    /// </summary>
    public ICollection<DocumentEntity> Documents { get; set; } = new List<DocumentEntity>();
}
