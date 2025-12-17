using Central.Domain.Documents;
using Central.Server.Features.Documents;

using Riok.Mapperly.Abstractions;

namespace Central.Server.Mappers;

/// <summary>
/// Static mapper for converting between Document domain model and DTOs.
/// </summary>
[Mapper]
public static partial class DocumentDtoMapper
{
    /// <summary>
    /// Maps a domain Document to DocumentDto.
    /// </summary>
    [MapProperty(nameof(Document.AddedBy), nameof(DocumentDto.AddedById))]
    [MapProperty(nameof(Document.UpdatedBy), nameof(DocumentDto.UpdatedById))]
    public static partial DocumentDto ToDto(this Document document);

    /// <summary>
    /// Maps a collection of Documents to DTOs.
    /// </summary>
    public static partial IReadOnlyCollection<DocumentDto> ToDto(this IEnumerable<Document> documents);

    /// <summary>
    /// Maps DocumentFile to DocumentFileDto.
    /// </summary>
    public static partial DocumentFileDto ToDto(this DocumentFile file);

    /// <summary>
    /// Maps DocumentFileDto to DocumentFile.
    /// </summary>
    public static partial DocumentFile ToDomain(this DocumentFileDto dto);
}