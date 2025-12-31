namespace Central.Domain.Contracts.Ports;

/// <summary>
/// Port for managing contract persistence.
/// </summary>
public interface IContractRepository
{
    /// <summary>
    /// Adds a new contract to the repository.
    /// </summary>
    /// <param name="contract">The contract to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added contract with generated ID.</returns>
    Task<Contract> AddAsync(Contract contract, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing contract.
    /// </summary>
    /// <param name="contract">The contract with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated contract.</returns>
    Task<Contract> UpdateAsync(Contract contract, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a contract by its identifier.
    /// </summary>
    /// <param name="id">The contract identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The contract if found; otherwise null.</returns>
    Task<Contract?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a contract by its name.
    /// </summary>
    /// <param name="name">The contract name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The contract if found; otherwise null.</returns>
    Task<Contract?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all contracts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all contracts.</returns>
    Task<IReadOnlyCollection<Contract>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a contract by its identifier.
    /// </summary>
    /// <param name="id">The contract identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the contract was deleted; otherwise false.</returns>
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a contract exists.
    /// </summary>
    /// <param name="id">The contract identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the contract exists; otherwise false.</returns>
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the number of documents associated with a contract.
    /// </summary>
    /// <param name="contractId">The contract identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of documents associated with the contract.</returns>
    Task<int> CountDocumentsAsync(long contractId, CancellationToken cancellationToken = default);
}