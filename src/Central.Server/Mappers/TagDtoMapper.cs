using Central.Domain.Tags;
using Central.Server.Features.Tags;
using Riok.Mapperly.Abstractions;

namespace Central.Server.Mappers;

/// <summary>
/// Static mapper for converting between Tag domain model and DTOs.
/// </summary>
[Mapper]
public static partial class TagDtoMapper
{
    /// <summary>
    /// Maps a domain Tag to TagDto.
    /// </summary>
    public static partial TagDto ToDto(this Tag tag);

    /// <summary>
    /// Maps a collection of Tags to DTOs.
    /// </summary>
    public static partial IReadOnlyCollection<TagDto> ToDto(this IEnumerable<Tag> tags);
}
