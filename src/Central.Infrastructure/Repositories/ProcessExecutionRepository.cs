using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Infrastructure.Entities;
using Central.Infrastructure.Mappers;
using Central.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository for managing process execution persistence and querying.
/// </summary>
public sealed class ProcessExecutionRepository : IProcessExecutionRepository
{
    private readonly ApplicationDbContext _context;

    public ProcessExecutionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProcessExecution?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ProcessExecutions
            .Include(e => e.Steps)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyCollection<ProcessExecution>> GetByDocumentIdAsync(
        long documentId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.ProcessExecutions
            .Include(e => e.Steps)
            .Where(e => e.DocumentId == documentId)
            .OrderByDescending(e => e.StartedAt)
            .ToListAsync(cancellationToken);

        return entities.ToDomain();
    }

    public async Task<IReadOnlyCollection<ProcessExecution>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.ProcessExecutions
            .Include(e => e.Steps)
            .OrderByDescending(e => e.StartedAt)
            .ToListAsync(cancellationToken);

        return entities.ToDomain();
    }

    public async Task<IReadOnlyCollection<ProcessExecution>> GetByStatusAsync(
        ExecutionStatus status,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.ProcessExecutions
            .Include(e => e.Steps)
            .Where(e => e.Status == status)
            .OrderBy(e => e.StartedAt)
            .ToListAsync(cancellationToken);

        return entities.ToDomain();
    }

    public async Task<ProcessExecution> CreateAsync(ProcessExecution processExecution, CancellationToken cancellationToken = default)
    {
        var entity = processExecution.ToEntity();
        entity.Id = 0; // Ensure EF generates new ID

        // Map steps
        entity.Steps = processExecution.Steps
            .Select(s =>
            {
                var stepEntity = s.ToEntity();
                stepEntity.Id = 0;
                stepEntity.ProcessExecutionId = 0; // Will be set by EF
                return stepEntity;
            })
            .ToList();

        _context.ProcessExecutions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        // Reload with steps
        var savedEntity = await _context.ProcessExecutions
            .Include(e => e.Steps)
            .FirstAsync(e => e.Id == entity.Id, cancellationToken);

        return savedEntity.ToDomain();
    }

    public async Task<ProcessExecution> UpdateAsync(ProcessExecution processExecution, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ProcessExecutions
            .Include(e => e.Steps)
            .FirstOrDefaultAsync(e => e.Id == processExecution.Id, cancellationToken);

        if (entity == null)
        {
            throw new InvalidOperationException($"ProcessExecution with ID {processExecution.Id} not found.");
        }

        processExecution.UpdateEntity(entity);

        // Update or add steps
        foreach (var step in processExecution.Steps)
        {
            var existingStepEntity = entity.Steps.FirstOrDefault(s => s.Id == step.Id);

            if (existingStepEntity != null)
            {
                // Update existing step
                step.UpdateEntity(existingStepEntity);
            }
            else
            {
                // Add new step
                var newStepEntity = step.ToEntity();
                newStepEntity.ProcessExecutionId = entity.Id;
                entity.Steps.Add(newStepEntity);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Reload with steps
        var updatedEntity = await _context.ProcessExecutions
            .Include(e => e.Steps)
            .FirstAsync(e => e.Id == entity.Id, cancellationToken);

        return updatedEntity.ToDomain();
    }
}