using Central.Domain.Ports;
using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services;

/// <summary>
/// Task executer that waits for a specified duration.
/// </summary>
public sealed class WaitTaskExecuter : ITaskExecuter
{
    private readonly ILogger<WaitTaskExecuter> _logger;

    public WaitTaskExecuter(ILogger<WaitTaskExecuter> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> ExecuteAsync(TaskExecutionContext context, CancellationToken cancellationToken = default)
    {
        if (context.WaitDurationSeconds == null || context.WaitDurationSeconds <= 0)
        {
            throw new InvalidOperationException(
                $"Wait task requires valid WaitDurationSeconds. Received: {context.WaitDurationSeconds}");
        }

        _logger.LogInformation(
            "Waiting {Seconds} seconds",
            context.WaitDurationSeconds.Value);

        await Task.Delay(
            TimeSpan.FromSeconds(context.WaitDurationSeconds.Value),
            cancellationToken);

        return $"{{\"status\": \"completed\", \"waitedSeconds\": {context.WaitDurationSeconds.Value}}}";
    }
}
