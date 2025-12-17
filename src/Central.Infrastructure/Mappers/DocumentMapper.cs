using Central.Domain.Documents;
using Central.Infrastructure.Entities;
using Riok.Mapperly.Abstractions;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Static mapper for converting between Document domain model and DocumentEntity.
/// </summary>
[Mapper]
public static partial class DocumentMapper
{
    /// <summary>
    /// Maps a domain Document to a DocumentEntity.
    /// </summary>
    [MapProperty(nameof(Document.OriginalFile) + "." + nameof(DocumentFile.FileName), nameof(DocumentEntity.OriginalFileName))]
    [MapProperty(nameof(Document.OriginalFile) + "." + nameof(DocumentFile.FilePath), nameof(DocumentEntity.OriginalFilePath))]
    [MapProperty(nameof(Document.ArchiveFile) + "." + nameof(DocumentFile.FileName), nameof(DocumentEntity.ArchiveFileName))]
    [MapProperty(nameof(Document.ArchiveFile) + "." + nameof(DocumentFile.FilePath), nameof(DocumentEntity.ArchiveFilePath))]
    [MapProperty(nameof(Document.Thumbnail) + "." + nameof(DocumentFile.FileName), nameof(DocumentEntity.ThumbnailFileName))]
    [MapProperty(nameof(Document.Thumbnail) + "." + nameof(DocumentFile.FilePath), nameof(DocumentEntity.ThumbnailFilePath))]
    [MapProperty(nameof(Document.AddedBy), nameof(DocumentEntity.AddedById))]
    [MapProperty(nameof(Document.UpdatedBy), nameof(DocumentEntity.UpdatedById))]
    [MapperIgnoreTarget(nameof(DocumentEntity.AddedBy))]
    [MapperIgnoreTarget(nameof(DocumentEntity.UpdatedBy))]
    [MapperIgnoreTarget(nameof(DocumentEntity.DocumentType))]
    [MapperIgnoreTarget(nameof(DocumentEntity.Tags))]
    public static partial DocumentEntity ToEntity(this Document document);

    /// <summary>
    /// Maps a DocumentEntity to a domain Document.
    /// </summary>
    [MapperIgnoreSource(nameof(DocumentEntity.OriginalFileName))]
    [MapperIgnoreSource(nameof(DocumentEntity.OriginalFilePath))]
    [MapperIgnoreSource(nameof(DocumentEntity.ArchiveFileName))]
    [MapperIgnoreSource(nameof(DocumentEntity.ArchiveFilePath))]
    [MapperIgnoreSource(nameof(DocumentEntity.ThumbnailFileName))]
    [MapperIgnoreSource(nameof(DocumentEntity.ThumbnailFilePath))]
    [MapperIgnoreSource(nameof(DocumentEntity.AddedBy))]
    [MapperIgnoreSource(nameof(DocumentEntity.UpdatedBy))]
    [MapperIgnoreSource(nameof(DocumentEntity.DocumentType))]
    [MapperIgnoreSource(nameof(DocumentEntity.Tags))]
    [MapProperty(nameof(DocumentEntity.AddedById), nameof(Document.AddedBy))]
    [MapProperty(nameof(DocumentEntity.UpdatedById), nameof(Document.UpdatedBy))]
    [MapperIgnoreTarget(nameof(Document.OriginalFile))]
    [MapperIgnoreTarget(nameof(Document.ArchiveFile))]
    [MapperIgnoreTarget(nameof(Document.Thumbnail))]
    [MapperIgnoreTarget(nameof(Document.TagIds))]
    private static partial Document ToDomainInternal(this DocumentEntity entity);

    /// <summary>
    /// Maps a DocumentEntity to a domain Document with file objects.
    /// </summary>
    public static Document ToDomain(this DocumentEntity entity)
    {
        var document = entity.ToDomainInternal();
        
        return document with
        {
            OriginalFile = MapToDocumentFile(entity.OriginalFileName, entity.OriginalFilePath),
            ArchiveFile = MapToDocumentFile(entity.ArchiveFileName, entity.ArchiveFilePath),
            Thumbnail = MapToDocumentFile(entity.ThumbnailFileName, entity.ThumbnailFilePath),
            TagIds = entity.Tags.Select(t => t.Id).ToList()
        };
    }

    /// <summary>
    /// Maps a collection of DocumentEntity to domain Documents.
    /// </summary>
    public static IReadOnlyCollection<Document> ToDomain(this IEnumerable<DocumentEntity> entities)
        => entities.Select(e => e.ToDomain()).ToList();

    /// <summary>
    /// Updates a DocumentEntity from a domain Document.
    /// </summary>
    [MapperIgnoreTarget(nameof(DocumentEntity.Id))]
    [MapperIgnoreTarget(nameof(DocumentEntity.Added))]
    [MapperIgnoreTarget(nameof(DocumentEntity.AddedBy))]
    [MapperIgnoreTarget(nameof(DocumentEntity.AddedById))]
    [MapperIgnoreTarget(nameof(DocumentEntity.UpdatedBy))]
    [MapperIgnoreTarget(nameof(DocumentEntity.DocumentType))]
    [MapperIgnoreTarget(nameof(DocumentEntity.Tags))]
    [MapProperty(nameof(Document.OriginalFile) + "." + nameof(DocumentFile.FileName), nameof(DocumentEntity.OriginalFileName))]
    [MapProperty(nameof(Document.OriginalFile) + "." + nameof(DocumentFile.FilePath), nameof(DocumentEntity.OriginalFilePath))]
    [MapProperty(nameof(Document.ArchiveFile) + "." + nameof(DocumentFile.FileName), nameof(DocumentEntity.ArchiveFileName))]
    [MapProperty(nameof(Document.ArchiveFile) + "." + nameof(DocumentFile.FilePath), nameof(DocumentEntity.ArchiveFilePath))]
    [MapProperty(nameof(Document.Thumbnail) + "." + nameof(DocumentFile.FileName), nameof(DocumentEntity.ThumbnailFileName))]
    [MapProperty(nameof(Document.Thumbnail) + "." + nameof(DocumentFile.FilePath), nameof(DocumentEntity.ThumbnailFilePath))]
    [MapProperty(nameof(Document.UpdatedBy), nameof(DocumentEntity.UpdatedById))]
    public static partial void UpdateEntity(Document document, DocumentEntity entity);

    private static DocumentFile? MapToDocumentFile(string? fileName, string? filePath)
    {
        if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(filePath))
            return null;

        return new DocumentFile(fileName, filePath);
    }
}
