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
        
        _context.Set<Entities.DocumentEntity>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDomain();
    }

    public async Task<Document> UpdateAsync(Document document, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<Entities.DocumentEntity>()
            .FindAsync([document.Id], cancellationToken);

        if (entity == null)
        {
            throw new InvalidOperationException($"Document with ID {document.Id} not found.");
        }

        DocumentMapper.UpdateEntity(document, entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDomain();
    }

    public async Task<Document?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<Entities.DocumentEntity>()
            .FindAsync([id], cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyCollection<Document>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Set<Entities.DocumentEntity>()
            .OrderByDescending(d => d.Added)
            .ToListAsync(cancellationToken);

        return entities.ToDomain();
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
