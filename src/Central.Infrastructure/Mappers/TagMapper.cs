using Central.Domain.Tags;
using Central.Infrastructure.Entities;

using Riok.Mapperly.Abstractions;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Static mapper for converting between Tag domain model and TagEntity.
/// </summary>
[Mapper]
public static partial class TagMapper
{
    /// <summary>
    /// Maps a domain Tag to a TagEntity.
    /// </summary>
    [MapperIgnoreTarget(nameof(TagEntity.Documents))]
    public static partial TagEntity ToEntity(this Tag tag);

    /// <summary>
    /// Maps a TagEntity to a domain Tag.
    /// </summary>
    [MapperIgnoreSource(nameof(TagEntity.Documents))]
    public static partial Tag ToDomain(this TagEntity entity);

    /// <summary>
    /// Maps a collection of TagEntity to domain Tags.
    /// </summary>
    public static IReadOnlyCollection<Tag> ToDomain(this IEnumerable<TagEntity> entities)
        => entities.Select(e => e.ToDomain()).ToList();

    /// <summary>
    /// Updates a TagEntity from a domain Tag.
    /// </summary>
    [MapperIgnoreTarget(nameof(TagEntity.Id))]
    [MapperIgnoreTarget(nameof(TagEntity.Created))]
    [MapperIgnoreTarget(nameof(TagEntity.Documents))]
    public static partial void UpdateEntity(Tag tag, TagEntity entity);
}