using Central.Domain.Correspondents;
using Central.Infrastructure.Entities;

using Riok.Mapperly.Abstractions;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Mapper for correspondent entities.
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class CorrespondentMapper
{
    /// <summary>
    /// Maps a correspondent entity to a domain correspondent.
    /// </summary>
    public static partial Correspondent ToDomain(this CorrespondentEntity entity);
    public static partial IQueryable<Correspondent> ToDomains(this IQueryable<CorrespondentEntity> queryable);

    /// <summary>
    /// Maps a domain correspondent to a correspondent entity.
    /// </summary>
    [MapperIgnoreTarget(nameof(CorrespondentEntity.Documents))]
    public static partial CorrespondentEntity ToEntity(this Correspondent correspondent);
}