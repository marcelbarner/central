using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Infrastructure.Entities;
using Central.Infrastructure.Mappers;
using Central.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository for managing task execution persistence.
/// </summary>
public sealed class TaskExecutionRepository : ITaskExecutionRepository
{
    private readonly ApplicationDbContext _context;

    public TaskExecutionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskExecution?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.TaskExecutions
            .FirstOrDefaultAsync(te => te.Id == id, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyCollection<TaskExecution>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.TaskExecutions
            .OrderByDescending(te => te.StartedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<IReadOnlyCollection<TaskExecution>> GetByDocumentIdAsync(
        long documentId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.TaskExecutions
            .Where(te => te.DocumentId == documentId)
            .OrderByDescending(te => te.StartedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<IReadOnlyCollection<TaskExecution>> GetByTaskIdAsync(
        long taskId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.TaskExecutions
            .Where(te => te.TaskId == taskId)
            .OrderByDescending(te => te.StartedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<TaskExecution> CreateAsync(TaskExecution execution, CancellationToken cancellationToken = default)
    {
        var entity = execution.ToEntity();
        entity.Id = 0; // Ensure EF generates new ID

        _context.TaskExecutions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDomain();
    }

    public async Task<TaskExecution> UpdateAsync(TaskExecution execution, CancellationToken cancellationToken = default)
    {
        var entity = await _context.TaskExecutions
            .FirstOrDefaultAsync(te => te.Id == execution.Id, cancellationToken);

        if (entity == null)
        {
            throw new InvalidOperationException($"TaskExecution with ID {execution.Id} not found.");
        }

        entity.Status = execution.Status;
        entity.StartedAt = execution.StartedAt;
        entity.CompletedAt = execution.CompletedAt;
        entity.ErrorMessage = execution.ErrorMessage;
        entity.Result = execution.Result;

        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDomain();
    }
}
