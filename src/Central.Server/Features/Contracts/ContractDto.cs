namespace Central.Server.Features.Contracts;

/// <summary>
/// Data transfer object for contract information.
/// </summary>
public sealed record ContractDto
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string State { get; init; }
    public long? CorrespondentId { get; init; }
    public string? CustomerId { get; init; }
    public string? ContractId { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required DateTimeOffset Updated { get; init; }
}