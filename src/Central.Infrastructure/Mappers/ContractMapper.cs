using Central.Domain.Contracts;
using Central.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Mappers;

/// <summary>
/// Extension methods for mapping between <see cref="Contract"/> and <see cref="ContractEntity"/>.
/// </summary>
public static class ContractMapper
{
    /// <summary>
    /// Maps a domain contract to a contract entity.
    /// </summary>
    public static ContractEntity ToEntity(this Contract contract)
    {
        return new ContractEntity
        {
            Id = contract.Id,
            Name = contract.Name,
            Description = contract.Description,
            State = (int)contract.State,
            CorrespondentId = contract.CorrespondentId,
            Created = contract.Created,
            Updated = contract.Updated
        };
    }

    /// <summary>
    /// Maps a contract entity to a domain contract.
    /// </summary>
    public static Contract ToDomain(this ContractEntity entity)
    {
        return new Contract
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            State = (ContractState)entity.State,
            CorrespondentId = entity.CorrespondentId,
            Created = entity.Created,
            Updated = entity.Updated
        };
    }

    /// <summary>
    /// Projects contract entities to domain contracts for queryable operations.
    /// </summary>
    public static IQueryable<Contract> ToDomains(this IQueryable<ContractEntity> entities)
    {
        return entities.Select(e => new Contract
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            State = (ContractState)e.State,
            CorrespondentId = e.CorrespondentId,
            Created = e.Created,
            Updated = e.Updated
        });
    }
}