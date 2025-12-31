namespace Central.Domain.Contracts;

/// <summary>
/// Represents a contract that can be associated with documents.
/// </summary>
public sealed record Contract
{
    /// <summary>
    /// Gets the unique identifier for the contract.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Gets the name of the contract.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the optional description of the contract.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the state of the contract.
    /// </summary>
    public required ContractState State { get; init; }

    /// <summary>
    /// Gets the ID of the correspondent associated with this contract.
    /// </summary>
    public long? CorrespondentId { get; init; }

    /// <summary>
    /// Gets the optional customer identifier.
    /// </summary>
    public string? CustomerId { get; init; }

    /// <summary>
    /// Gets the optional contract identifier.
    /// </summary>
    public string? ContractId { get; init; }

    /// <summary>
    /// Gets the timestamp when the contract was created.
    /// </summary>
    public required DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the timestamp when the contract was last updated.
    /// </summary>
    public required DateTimeOffset Updated { get; init; }
}