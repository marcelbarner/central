using Central.Domain.Contracts;
using Central.Server.Features.Contracts;

namespace Central.Server.Mappers;

/// <summary>
/// Extension methods for mapping between <see cref="Contract"/> and <see cref="ContractDto"/>.
/// </summary>
public static class ContractMapper
{
    /// <summary>
    /// Maps a domain contract to a DTO.
    /// </summary>
    public static ContractDto ToDto(this Contract contract)
    {
        return new ContractDto
        {
            Id = contract.Id,
            Name = contract.Name,
            Description = contract.Description,
            State = contract.State.ToString(),
            CorrespondentId = contract.CorrespondentId,
            Created = contract.Created,
            Updated = contract.Updated
        };
    }
}