namespace Central.Domain.Contracts.Services;

/// <summary>
/// Service for managing contract operations.
/// </summary>
public interface IContractService
{
    /// <summary>
    /// Creates a new contract.
    /// </summary>
    /// <param name="name">The name of the contract.</param>
    /// <param name="description">The optional description of the contract.</param>
    /// <param name="state">The initial state of the contract.</param>
    /// <param name="correspondentId">The optional correspondent ID.</param>
    /// <param name="customerId">The optional customer identifier.</param>
    /// <param name="contractId">The optional contract identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created contract.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a contract with the same name already exists.</exception>
    Task<Contract> CreateAsync(
        string name,
        string? description,
        ContractState state,
        long? correspondentId,
        string? customerId,
        string? contractId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing contract.
    /// </summary>
    /// <param name="id">The contract ID.</param>
    /// <param name="name">The updated name.</param>
    /// <param name="description">The updated description.</param>
    /// <param name="state">The updated state.</param>
    /// <param name="correspondentId">The updated correspondent ID.</param>
    /// <param name="customerId">The updated customer identifier.</param>
    /// <param name="contractId">The updated contract identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated contract.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the contract does not exist or name conflict occurs.</exception>
    Task<Contract> UpdateAsync(
        long id,
        string name,
        string? description,
        ContractState state,
        long? correspondentId,
        string? customerId,
        string? contractId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a contract by ID.
    /// </summary>
    /// <param name="id">The contract ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The contract if found; otherwise null.</returns>
    Task<Contract?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all contracts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all contracts.</returns>
    Task<IReadOnlyCollection<Contract>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a contract.
    /// </summary>
    /// <param name="id">The contract ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully; otherwise false.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the contract has associated documents.</exception>
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}