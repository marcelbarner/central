using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Infrastructure.Entities;
using Central.Infrastructure.Mappers;
using Central.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository for managing pipeline execution persistence.
/// </summary>
public sealed class PipelineExecutionRepository : IPipelineExecutionRepository
{
    private readonly ApplicationDbContext _context;

    public PipelineExecutionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PipelineExecution?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PipelineExecutions
            .Include(pe => pe.TaskExecutions)
            .FirstOrDefaultAsync(pe => pe.Id == id, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyCollection<PipelineExecution>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.PipelineExecutions
            .Include(pe => pe.TaskExecutions)
            .OrderByDescending(pe => pe.StartedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<IReadOnlyCollection<PipelineExecution>> GetByDocumentIdAsync(
        long documentId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.PipelineExecutions
            .Include(pe => pe.TaskExecutions)
            .Where(pe => pe.DocumentId == documentId)
            .OrderByDescending(pe => pe.StartedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<PipelineExecution> CreateAsync(PipelineExecution execution, CancellationToken cancellationToken = default)
    {
        var entity = execution.ToEntity();
        entity.Id = 0; // Ensure EF generates new ID

        _context.PipelineExecutions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDomain();
    }

    public async Task<PipelineExecution> UpdateAsync(PipelineExecution execution, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PipelineExecutions
            .Include(pe => pe.TaskExecutions)
            .FirstOrDefaultAsync(pe => pe.Id == execution.Id, cancellationToken);

        if (entity == null)
        {
            throw new InvalidOperationException($"PipelineExecution with ID {execution.Id} not found.");
        }

        entity.Status = execution.Status;
        entity.StartedAt = execution.StartedAt;
        entity.CompletedAt = execution.CompletedAt;
        entity.ErrorMessage = execution.ErrorMessage;

        await _context.SaveChangesAsync(cancellationToken);

        // Reload with task executions
        var savedEntity = await _context.PipelineExecutions
            .Include(pe => pe.TaskExecutions)
            .FirstAsync(pe => pe.Id == entity.Id, cancellationToken);

        return savedEntity.ToDomain();
    }
}
