using Central.Domain.DocumentTypes;
using Central.Server.Features.DocumentTypes;
using Riok.Mapperly.Abstractions;

namespace Central.Server.Mappers;

/// <summary>
/// Static mapper for converting between DocumentType domain model and DTOs.
/// </summary>
[Mapper]
public static partial class DocumentTypeDtoMapper
{
    /// <summary>
    /// Maps a domain DocumentType to DocumentTypeDto.
    /// </summary>
    public static partial DocumentTypeDto ToDto(this DocumentType documentType);

    /// <summary>
    /// Maps a collection of DocumentTypes to DTOs.
    /// </summary>
    public static partial IReadOnlyCollection<DocumentTypeDto> ToDto(this IEnumerable<DocumentType> documentTypes);
}
