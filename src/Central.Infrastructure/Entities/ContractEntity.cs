namespace Central.Infrastructure.Entities;

/// <summary>
/// Entity representing a contract in the database.
/// </summary>
public class ContractEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the contract name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the contract description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the contract state.
    /// </summary>
    public int State { get; set; }

    /// <summary>
    /// Gets or sets the correspondent ID associated with this contract.
    /// </summary>
    public long? CorrespondentId { get; set; }

    /// <summary>
    /// Gets or sets the correspondent associated with this contract.
    /// </summary>
    public CorrespondentEntity? Correspondent { get; set; }

    /// <summary>
    /// Gets or sets the optional customer identifier.
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the optional contract identifier.
    /// </summary>
    public string? ContractId { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Gets or sets the documents associated with this contract.
    /// </summary>
    public ICollection<DocumentEntity> Documents { get; set; } = [];
}