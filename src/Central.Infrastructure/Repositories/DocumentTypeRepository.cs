using Central.Domain.DocumentTypes;
using Central.Domain.DocumentTypes.Ports;
using Central.Infrastructure.Mappers;
using Central.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository for managing document type persistence.
/// </summary>
public sealed class DocumentTypeRepository : IDocumentTypeRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentType> AddAsync(DocumentType documentType, CancellationToken cancellationToken = default)
    {
        var entity = documentType.ToEntity();
        entity.Id = 0; // Ensure EF generates new ID

        _context.Set<Entities.DocumentTypeEntity>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDomain();
    }

    public async Task<DocumentType> UpdateAsync(DocumentType documentType, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<Entities.DocumentTypeEntity>()
            .FindAsync([documentType.Id], cancellationToken);

        if (entity == null)
        {
            throw new InvalidOperationException($"Document type with ID {documentType.Id} not found.");
        }

        DocumentTypeMapper.UpdateEntity(documentType, entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDomain();
    }

    public async Task<DocumentType?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<Entities.DocumentTypeEntity>()
            .FindAsync([id], cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<DocumentType?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<Entities.DocumentTypeEntity>()
            .FirstOrDefaultAsync(dt => dt.Name == name, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyCollection<DocumentType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Set<Entities.DocumentTypeEntity>()
            .OrderBy(dt => dt.Name)
            .ToListAsync(cancellationToken);

        return entities.ToDomain();
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<Entities.DocumentTypeEntity>()
            .FindAsync([id], cancellationToken);

        if (entity != null)
        {
            _context.Set<Entities.DocumentTypeEntity>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(string name, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Entities.DocumentTypeEntity>().Where(dt => dt.Name == name);

        if (excludeId.HasValue)
        {
            query = query.Where(dt => dt.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}