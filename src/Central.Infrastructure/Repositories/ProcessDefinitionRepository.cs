using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Infrastructure.Entities;
using Central.Infrastructure.Mappers;
using Central.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository for managing process definition persistence.
/// </summary>
public sealed class ProcessDefinitionRepository : IProcessDefinitionRepository
{
    private readonly ApplicationDbContext _context;

    public ProcessDefinitionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProcessDefinition?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ProcessDefinitions
            .Include(p => p.Steps)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyCollection<ProcessDefinition>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.ProcessDefinitions
            .Include(p => p.Steps)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return entities.ToDomain();
    }

    public async Task<IReadOnlyCollection<ProcessDefinition>> GetEnabledByTriggerStateAsync(
        DocumentState triggerState,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.ProcessDefinitions
            .Include(p => p.Steps)
            .Where(p => p.Enabled && p.TriggerState == triggerState)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return entities.ToDomain();
    }

    public async Task<ProcessDefinition> CreateAsync(ProcessDefinition processDefinition, CancellationToken cancellationToken = default)
    {
        var entity = processDefinition.ToEntity();
        entity.Id = 0; // Ensure EF generates new ID

        // Map steps
        entity.Steps = processDefinition.Steps
            .Select(s =>
            {
                var stepEntity = s.ToEntity();
                stepEntity.Id = 0;
                stepEntity.ProcessDefinitionId = 0; // Will be set by EF
                return stepEntity;
            })
            .ToList();

        _context.ProcessDefinitions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        // Reload with steps
        var savedEntity = await _context.ProcessDefinitions
            .Include(p => p.Steps)
            .FirstAsync(p => p.Id == entity.Id, cancellationToken);

        return savedEntity.ToDomain();
    }

    public async Task<ProcessDefinition> UpdateAsync(ProcessDefinition processDefinition, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ProcessDefinitions
            .Include(p => p.Steps)
            .FirstOrDefaultAsync(p => p.Id == processDefinition.Id, cancellationToken);

        if (entity == null)
        {
            throw new InvalidOperationException($"ProcessDefinition with ID {processDefinition.Id} not found.");
        }

        processDefinition.UpdateEntity(entity);

        // Update steps - remove old, add new
        entity.Steps.Clear();
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var step in processDefinition.Steps)
        {
            var stepEntity = step.ToEntity();
            stepEntity.ProcessDefinitionId = entity.Id;
            stepEntity.Id = 0; // Let EF Core assign new ID
            entity.Steps.Add(stepEntity);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Reload with steps
        var updatedEntity = await _context.ProcessDefinitions
            .Include(p => p.Steps)
            .FirstAsync(p => p.Id == entity.Id, cancellationToken);

        return updatedEntity.ToDomain();
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ProcessDefinitions
            .Include(p => p.Steps)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (entity != null)
        {
            _context.ProcessDefinitions.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}