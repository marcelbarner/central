using Central.Domain.Correspondents.Ports;

namespace Central.Domain.Correspondents.Services;

/// <summary>
/// Domain service for managing correspondents.
/// </summary>
public class CorrespondentService(ICorrespondentRepository repository) : ICorrespondentService
{
    /// <inheritdoc />
    public async Task<Correspondent> CreateAsync(string name, string? description = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (await repository.ExistsAsync(name, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException($"A correspondent with the name '{name}' already exists.");
        }

        var correspondent = new Correspondent
        {
            Name = name,
            Description = description,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow
        };

        return await repository.AddAsync(correspondent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Correspondent> UpdateAsync(long id, string name, string? description = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var existing = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Correspondent with ID {id} not found.");

        if (await repository.ExistsAsync(name, excludeId: id, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException($"A correspondent with the name '{name}' already exists.");
        }

        var updated = existing with
        {
            Name = name,
            Description = description,
            Updated = DateTimeOffset.UtcNow
        };

        return await repository.UpdateAsync(updated, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Correspondent?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return repository.GetByIdAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<Correspondent>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetAllAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Correspondent with ID {id} not found.");

        await repository.DeleteAsync(id, cancellationToken);
    }
}
