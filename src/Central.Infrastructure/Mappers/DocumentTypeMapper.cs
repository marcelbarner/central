using Central.Domain.DocumentTypes;
using Central.Infrastructure.Entities;
using Riok.Mapperly.Abstractions;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Static mapper for converting between DocumentType domain model and DocumentTypeEntity.
/// </summary>
[Mapper]
public static partial class DocumentTypeMapper
{
    /// <summary>
    /// Maps a domain DocumentType to a DocumentTypeEntity.
    /// </summary>
    [MapperIgnoreTarget(nameof(DocumentTypeEntity.Documents))]
    public static partial DocumentTypeEntity ToEntity(this DocumentType documentType);

    /// <summary>
    /// Maps a DocumentTypeEntity to a domain DocumentType.
    /// </summary>
    [MapperIgnoreSource(nameof(DocumentTypeEntity.Documents))]
    public static partial DocumentType ToDomain(this DocumentTypeEntity entity);

    /// <summary>
    /// Maps a collection of DocumentTypeEntity to domain DocumentTypes.
    /// </summary>
    public static IReadOnlyCollection<DocumentType> ToDomain(this IEnumerable<DocumentTypeEntity> entities)
        => entities.Select(e => e.ToDomain()).ToList();

    /// <summary>
    /// Updates a DocumentTypeEntity from a domain DocumentType.
    /// </summary>
    [MapperIgnoreTarget(nameof(DocumentTypeEntity.Id))]
    [MapperIgnoreTarget(nameof(DocumentTypeEntity.Created))]
    [MapperIgnoreTarget(nameof(DocumentTypeEntity.Documents))]
    public static partial void UpdateEntity(DocumentType documentType, DocumentTypeEntity entity);
}
