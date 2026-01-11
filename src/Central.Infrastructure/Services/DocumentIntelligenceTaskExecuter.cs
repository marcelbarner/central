using Central.Domain.Ports;
using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services;

/// <summary>
/// Task executer for Azure Document Intelligence tasks.
/// </summary>
public sealed class DocumentIntelligenceTaskExecuter : ITaskExecuter
{
    private readonly ILogger<DocumentIntelligenceTaskExecuter> _logger;

    public DocumentIntelligenceTaskExecuter(ILogger<DocumentIntelligenceTaskExecuter> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> ExecuteAsync(TaskExecutionContext context, CancellationToken cancellationToken = default)
    {
        // TODO: Implement Azure Document Intelligence SDK integration
        // This is a placeholder for the actual implementation
        _logger.LogInformation(
            "Executing Azure Document Intelligence task '{TaskName}' with model: {Model}",
            context.Task.Name,
            context.Task.Configuration.AzureModelOrDeployment);

        await Task.Delay(100, cancellationToken); // Simulate API call

        return "{\"status\": \"success\", \"message\": \"Azure Document Intelligence task executed (placeholder)\"}";
    }
}
