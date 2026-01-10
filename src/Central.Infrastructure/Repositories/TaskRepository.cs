using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Infrastructure.Entities;
using Central.Infrastructure.Mappers;
using Central.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository for managing task persistence.
/// </summary>
public sealed class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProcessingTask?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyCollection<ProcessingTask>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Tasks
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<IReadOnlyCollection<ProcessingTask>> GetEnabledAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Tasks
            .Where(t => t.Enabled)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<ProcessingTask> CreateAsync(ProcessingTask task, CancellationToken cancellationToken = default)
    {
        var entity = task.ToEntity();
        entity.Id = 0; // Ensure EF generates new ID

        _context.Tasks.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDomain();
    }

    public async Task<ProcessingTask> UpdateAsync(ProcessingTask task, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken);

        if (entity == null)
        {
            throw new InvalidOperationException($"Task with ID {task.Id} not found.");
        }

        entity.UpdateEntity(task);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDomain();
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (entity != null)
        {
            _context.Tasks.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
