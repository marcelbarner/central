namespace Central.Domain.Documents.Services;

/// <summary>
/// Service for executing document processing workflows.
/// </summary>
public interface IProcessExecutionService
{
    /// <summary>
    /// Creates and executes a process for a document.
    /// </summary>
    /// <param name="processDefinitionId">The ID of the process definition to execute.</param>
    /// <param name="documentId">The ID of the document to process.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The process execution result.</returns>
    Task<ProcessExecution> ExecuteProcessAsync(
        long processDefinitionId,
        long documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds documents matching the trigger state of enabled processes and creates executions for them.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of process executions created.</returns>
    Task<int> ProcessPendingDocumentsAsync(CancellationToken cancellationToken = default);
}