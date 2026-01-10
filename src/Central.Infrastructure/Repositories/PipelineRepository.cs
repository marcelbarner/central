using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Infrastructure.Entities;
using Central.Infrastructure.Mappers;
using Central.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository for managing pipeline persistence.
/// </summary>
public sealed class PipelineRepository : IPipelineRepository
{
    private readonly ApplicationDbContext _context;

    public PipelineRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Pipeline?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Pipelines
            .Include(p => p.Steps)
                .ThenInclude(s => s.Task)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyCollection<Pipeline>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Pipelines
            .Include(p => p.Steps)
                .ThenInclude(s => s.Task)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<IReadOnlyCollection<Pipeline>> GetEnabledByTriggerStateAsync(
        DocumentState triggerState,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Pipelines
            .Include(p => p.Steps)
                .ThenInclude(s => s.Task)
            .Where(p => p.Enabled && p.TriggerState == triggerState)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<Pipeline> CreateAsync(Pipeline pipeline, CancellationToken cancellationToken = default)
    {
        var entity = pipeline.ToEntity();
        entity.Id = 0; // Ensure EF generates new ID

        // Map steps
        entity.Steps = pipeline.Steps
            .Select(s =>
            {
                var stepEntity = s.ToEntity();
                stepEntity.Id = 0;
                stepEntity.PipelineId = 0; // Will be set by EF
                return stepEntity;
            })
            .ToList();

        _context.Pipelines.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        // Reload with steps
        var savedEntity = await _context.Pipelines
            .Include(p => p.Steps)
                .ThenInclude(s => s.Task)
            .FirstAsync(p => p.Id == entity.Id, cancellationToken);

        return savedEntity.ToDomain();
    }

    public async Task<Pipeline> UpdateAsync(Pipeline pipeline, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Pipelines
            .Include(p => p.Steps)
            .FirstOrDefaultAsync(p => p.Id == pipeline.Id, cancellationToken);

        if (entity == null)
        {
            throw new InvalidOperationException($"Pipeline with ID {pipeline.Id} not found.");
        }

        entity.UpdateEntity(pipeline);

        // Update steps - remove old ones, add new ones
        _context.PipelineSteps.RemoveRange(entity.Steps);

        entity.Steps = pipeline.Steps
            .Select(s =>
            {
                var stepEntity = s.ToEntity();
                stepEntity.Id = 0;
                stepEntity.PipelineId = pipeline.Id;
                return stepEntity;
            })
            .ToList();

        await _context.SaveChangesAsync(cancellationToken);

        // Reload with steps
        var savedEntity = await _context.Pipelines
            .Include(p => p.Steps)
                .ThenInclude(s => s.Task)
            .FirstAsync(p => p.Id == entity.Id, cancellationToken);

        return savedEntity.ToDomain();
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Pipelines
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (entity != null)
        {
            _context.Pipelines.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
