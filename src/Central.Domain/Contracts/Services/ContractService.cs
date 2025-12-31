using Central.Domain.Contracts.Ports;

namespace Central.Domain.Contracts.Services;

/// <summary>
/// Implementation of contract service.
/// </summary>
public class ContractService(IContractRepository contractRepository) : IContractService
{
    /// <inheritdoc />
    public async Task<Contract> CreateAsync(
        string name,
        string? description,
        ContractState state,
        long? correspondentId,
        string? customerId,
        string? contractId,
        CancellationToken cancellationToken = default)
    {
        // Check if a contract with the same name already exists
        var existing = await contractRepository.GetByNameAsync(name, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException($"A contract with the name '{name}' already exists.");
        }

        var now = DateTimeOffset.UtcNow;
        var contract = new Contract
        {
            Name = name,
            Description = description,
            State = state,
            CorrespondentId = correspondentId,
            CustomerId = customerId,
            ContractId = contractId,
            Created = now,
            Updated = now
        };

        return await contractRepository.AddAsync(contract, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Contract> UpdateAsync(
        long id,
        string name,
        string? description,
        ContractState state,
        long? correspondentId,
        string? customerId,
        string? contractId,
        CancellationToken cancellationToken = default)
    {
        var existing = await contractRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            throw new InvalidOperationException($"Contract with ID {id} not found.");
        }

        // Check if another contract has the same name
        var duplicateName = await contractRepository.GetByNameAsync(name, cancellationToken);
        if (duplicateName is not null && duplicateName.Id != id)
        {
            throw new InvalidOperationException($"A contract with the name '{name}' already exists.");
        }

        var updated = existing with
        {
            Name = name,
            Description = description,
            State = state,
            CorrespondentId = correspondentId,
            CustomerId = customerId,
            ContractId = contractId,
            Updated = DateTimeOffset.UtcNow
        };

        return await contractRepository.UpdateAsync(updated, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Contract?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return contractRepository.GetByIdAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<Contract>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return contractRepository.GetAllAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        // Check if contract has associated documents
        var documentCount = await contractRepository.CountDocumentsAsync(id, cancellationToken);
        if (documentCount > 0)
        {
            throw new InvalidOperationException(
                $"Cannot delete contract with ID {id} because it has {documentCount} associated document(s).");
        }

        return await contractRepository.DeleteAsync(id, cancellationToken);
    }
}