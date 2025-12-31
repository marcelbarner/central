using Central.Domain.Contracts;
using Central.Domain.Contracts.Ports;
using Central.Infrastructure.Mappers;
using Central.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for contracts.
/// </summary>
public class ContractRepository(ApplicationDbContext context) : IContractRepository
{
    /// <inheritdoc />
    public async Task<Contract> AddAsync(Contract contract, CancellationToken cancellationToken = default)
    {
        var entity = contract.ToEntity();
        await context.Contracts.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entity.ToDomain();
    }

    /// <inheritdoc />
    public async Task<Contract> UpdateAsync(Contract contract, CancellationToken cancellationToken = default)
    {
        var entity = contract.ToEntity();
        context.Contracts.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity.ToDomain();
    }

    /// <inheritdoc />
    public async Task<Contract?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.Contracts.ToDomains()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Contract?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.Contracts.ToDomains()
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Contract>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Contracts.ToDomains()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Contracts.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return false;
        }

        context.Contracts.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.Contracts.AnyAsync(c => c.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountDocumentsAsync(long contractId, CancellationToken cancellationToken = default)
    {
        return await context.Documents
            .Where(d => d.ContractId == contractId)
            .CountAsync(cancellationToken);
    }
}