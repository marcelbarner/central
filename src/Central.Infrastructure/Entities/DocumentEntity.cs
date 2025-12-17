using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Central.Domain.Users;

namespace Central.Infrastructure.Entities;

/// <summary>
/// Entity representing a document in the database.
/// </summary>
[Table("Documents")]
public sealed class DocumentEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the document.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date associated with the document content.
    /// </summary>
    public DateTimeOffset? DocumentDate { get; set; }

    /// <summary>
    /// Gets or sets the textual content or description.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the original file name.
    /// </summary>
    [MaxLength(500)]
    public string? OriginalFileName { get; set; }

    /// <summary>
    /// Gets or sets the original file path.
    /// </summary>
    [MaxLength(2000)]
    public string? OriginalFilePath { get; set; }

    /// <summary>
    /// Gets or sets the archive file name.
    /// </summary>
    [MaxLength(500)]
    public string? ArchiveFileName { get; set; }

    /// <summary>
    /// Gets or sets the archive file path.
    /// </summary>
    [MaxLength(2000)]
    public string? ArchiveFilePath { get; set; }

    /// <summary>
    /// Gets or sets the thumbnail file name.
    /// </summary>
    [MaxLength(500)]
    public string? ThumbnailFileName { get; set; }

    /// <summary>
    /// Gets or sets the thumbnail file path.
    /// </summary>
    [MaxLength(2000)]
    public string? ThumbnailFilePath { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the document was added.
    /// </summary>
    [Required]
    public DateTimeOffset Added { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the document was last updated.
    /// </summary>
    [Required]
    public DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the user who added the document.
    /// </summary>
    public long? AddedById { get; set; }

    /// <summary>
    /// Gets or sets the user who added the document.
    /// </summary>
    [ForeignKey(nameof(AddedById))]
    public User? AddedBy { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the user who last updated the document.
    /// </summary>
    public long? UpdatedById { get; set; }

    /// <summary>
    /// Gets or sets the user who last updated the document.
    /// </summary>
    [ForeignKey(nameof(UpdatedById))]
    public User? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the document type.
    /// </summary>
    public long? DocumentTypeId { get; set; }

    /// <summary>
    /// Gets or sets the document type.
    /// </summary>
    [ForeignKey(nameof(DocumentTypeId))]
    public DocumentTypeEntity? DocumentType { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the correspondent.
    /// </summary>
    public long? CorrespondentId { get; set; }

    /// <summary>
    /// Gets or sets the correspondent.
    /// </summary>
    [ForeignKey(nameof(CorrespondentId))]
    public CorrespondentEntity? Correspondent { get; set; }

    /// <summary>
    /// Navigation property for tags associated with this document.
    /// </summary>
    public ICollection<TagEntity> Tags { get; set; } = new List<TagEntity>();
}