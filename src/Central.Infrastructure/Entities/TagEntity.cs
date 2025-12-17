using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Central.Infrastructure.Entities;

/// <summary>
/// Entity representing a tag in the database.
/// </summary>
[Table("Tags")]
public sealed class TagEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the tag.
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
    /// Gets or sets the timestamp when the tag was created.
    /// </summary>
    [Required]
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the tag was last updated.
    /// </summary>
    [Required]
    public DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Navigation property for documents that have this tag.
    /// </summary>
    public ICollection<DocumentEntity> Documents { get; set; } = new List<DocumentEntity>();
}
