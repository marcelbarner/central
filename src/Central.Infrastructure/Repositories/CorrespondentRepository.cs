using Central.Domain.Correspondents;
using Central.Domain.Correspondents.Ports;
using Central.Infrastructure.Mappers;
using Central.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for correspondents.
/// </summary>
public class CorrespondentRepository(ApplicationDbContext context) : ICorrespondentRepository
{
    /// <inheritdoc />
    public async Task<Correspondent> AddAsync(Correspondent correspondent, CancellationToken cancellationToken = default)
    {
        var entity = correspondent.ToEntity();
        await context.Correspondents.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entity.ToDomain();
    }

    /// <inheritdoc />
    public async Task<Correspondent> UpdateAsync(Correspondent correspondent, CancellationToken cancellationToken = default)
    {
        var entity = correspondent.ToEntity();
        context.Correspondents.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity.ToDomain();
    }

    /// <inheritdoc />
    public async Task<Correspondent?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Correspondents
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return entity?.ToDomain();
    }

    /// <inheritdoc />
    public async Task<Correspondent?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = await context.Correspondents
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
        return entity?.ToDomain();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Correspondent>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context.Correspondents
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
        return entities.Select(e => e.ToDomain()).ToList();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Correspondents.FindAsync([id], cancellationToken);
        if (entity != null)
        {
            context.Correspondents.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string name, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await context.Correspondents
            .AsNoTracking()
            .AnyAsync(c => c.Name == name && (excludeId == null || c.Id != excludeId), cancellationToken);
    }
}