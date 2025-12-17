using Central.Domain.Tags;
using Central.Domain.Tags.Ports;
using Central.Infrastructure.Mappers;
using Central.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository for managing tag persistence.
/// </summary>
public sealed class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _context;

    public TagRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Tag> AddAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        var entity = tag.ToEntity();
        entity.Id = 0; // Ensure EF generates new ID

        _context.Set<Entities.TagEntity>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDomain();
    }

    public async Task<Tag> UpdateAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<Entities.TagEntity>()
            .FindAsync([tag.Id], cancellationToken);

        if (entity == null)
        {
            throw new InvalidOperationException($"Tag with ID {tag.Id} not found.");
        }

        TagMapper.UpdateEntity(tag, entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDomain();
    }

    public async Task<Tag?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<Entities.TagEntity>()
            .FindAsync([id], cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<Entities.TagEntity>()
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyCollection<Tag>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Set<Entities.TagEntity>()
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        return entities.ToDomain();
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<Entities.TagEntity>()
            .FindAsync([id], cancellationToken);

        if (entity != null)
        {
            _context.Set<Entities.TagEntity>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(string name, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Entities.TagEntity>().Where(t => t.Name == name);

        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}