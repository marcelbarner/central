namespace Central.Domain.Documents.Ports;

/// <summary>
/// Repository for managing process definition persistence.
/// </summary>
public interface IProcessDefinitionRepository
{
    /// <summary>
    /// Gets a process definition by its unique identifier.
    /// </summary>
    /// <param name="id">The process definition ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The process definition, or null if not found.</returns>
    Task<ProcessDefinition?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all process definitions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all process definitions.</returns>
    Task<IReadOnlyCollection<ProcessDefinition>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all enabled process definitions that trigger on the specified document state.
    /// </summary>
    /// <param name="triggerState">The document state that triggers the process.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of matching enabled process definitions.</returns>
    Task<IReadOnlyCollection<ProcessDefinition>> GetEnabledByTriggerStateAsync(
        DocumentState triggerState,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new process definition.
    /// </summary>
    /// <param name="processDefinition">The process definition to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created process definition with generated ID.</returns>
    Task<ProcessDefinition> CreateAsync(ProcessDefinition processDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing process definition.
    /// </summary>
    /// <param name="processDefinition">The process definition to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated process definition.</returns>
    Task<ProcessDefinition> UpdateAsync(ProcessDefinition processDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a process definition by its ID.
    /// </summary>
    /// <param name="id">The process definition ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}