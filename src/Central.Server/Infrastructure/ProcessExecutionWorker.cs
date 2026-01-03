using Central.Domain.Documents.Services;

namespace Central.Server.Infrastructure;

/// <summary>
/// Background service that periodically checks for documents ready for processing
/// and executes configured process definitions.
/// </summary>
public sealed class ProcessExecutionWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProcessExecutionWorker> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30); // Configurable interval

    public ProcessExecutionWorker(
        IServiceProvider serviceProvider,
        ILogger<ProcessExecutionWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ProcessExecutionWorker starting. Check interval: {Interval}", _interval);

        // Wait a bit for the application to fully start
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessDocumentsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing documents");
            }

            // Wait for the next interval
            try
            {
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when stopping
                break;
            }
        }

        _logger.LogInformation("ProcessExecutionWorker stopping");
    }

    private async Task ProcessDocumentsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var processExecutionService = scope.ServiceProvider.GetRequiredService<IProcessExecutionService>();

        try
        {
            var executionCount = await processExecutionService.ProcessPendingDocumentsAsync(cancellationToken);

            if (executionCount > 0)
            {
                _logger.LogInformation("Processed {ExecutionCount} documents", executionCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process pending documents");
        }
    }
}