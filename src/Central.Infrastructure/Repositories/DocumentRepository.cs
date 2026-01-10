using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Infrastructure.Mappers;
using Central.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository for managing document persistence.
/// </summary>
public sealed class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Document> AddAsync(Document document, CancellationToken cancellationToken = default)
    {
        var entity = document.ToEntity();
        entity.Id = 0; // Ensure EF generates new ID

        // Load and assign tag entities
        if (document.TagIds.Any())
        {
            var tags = await _context.Set<Entities.TagEntity>()
                .Where(t => document.TagIds.Contains(t.Id))
                .ToListAsync(cancellationToken);

            entity.Tags = tags;
        }

        _context.Set<Entities.DocumentEntity>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        // Reload with tags for response
        var savedEntity = await _context.Set<Entities.DocumentEntity>()
            .Include(d => d.Tags)
            .FirstAsync(d => d.Id == entity.Id, cancellationToken);

        return savedEntity.ToDomain();
    }

    public async Task<Document> UpdateAsync(Document document, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<Entities.DocumentEntity>()
            .Include(d => d.Tags)
            .FirstOrDefaultAsync(d => d.Id == document.Id, cancellationToken);

        if (entity == null)
        {
            throw new InvalidOperationException($"Document with ID {document.Id} not found.");
        }

        DocumentMapper.UpdateEntity(document, entity);

        // Update tag associations
        entity.Tags.Clear();
        if (document.TagIds.Any())
        {
            var tags = await _context.Set<Entities.TagEntity>()
                .Where(t => document.TagIds.Contains(t.Id))
                .ToListAsync(cancellationToken);

            foreach (var tag in tags)
            {
                entity.Tags.Add(tag);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDomain();
    }

    public async Task<Document?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<Entities.DocumentEntity>()
            .Include(d => d.Tags)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyCollection<Document>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Set<Entities.DocumentEntity>()
            .Include(d => d.Tags)
            .OrderByDescending(d => d.Added)
            .ToListAsync(cancellationToken);

        return entities.ToDomain();
    }
    public async Task<IReadOnlyCollection<Document>> GetByStateAsync(DocumentState state, CancellationToken cancellationToken = default)
    {
        var stateValue = (int)state;
        var entities = await _context.Set<Entities.DocumentEntity>()
            .Include(d => d.Tags)
            .Where(d => d.State == stateValue)
            .OrderByDescending(d => d.Added)
            .ToListAsync(cancellationToken);

        return entities.Select(e => e.ToDomain()).ToList();
    }
    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<Entities.DocumentEntity>()
            .FindAsync([id], cancellationToken);

        if (entity != null)
        {
            _context.Set<Entities.DocumentEntity>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}