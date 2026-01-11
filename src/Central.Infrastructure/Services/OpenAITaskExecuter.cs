using Central.Domain.Ports;
using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services;

/// <summary>
/// Task executer for Azure OpenAI tasks.
/// </summary>
public sealed class OpenAITaskExecuter : ITaskExecuter
{
    private readonly ILogger<OpenAITaskExecuter> _logger;

    public OpenAITaskExecuter(ILogger<OpenAITaskExecuter> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> ExecuteAsync(TaskExecutionContext context, CancellationToken cancellationToken = default)
    {
        // TODO: Implement Azure OpenAI SDK integration
        // This is a placeholder for the actual implementation
        _logger.LogInformation(
            "Executing Azure OpenAI task '{TaskName}' with deployment: {Deployment}, prompt: {Prompt}",
            context.Task.Name,
            context.Task.Configuration.AzureModelOrDeployment,
            context.Task.Configuration.Prompt?.Length > 50
                ? context.Task.Configuration.Prompt[..50] + "..."
                : context.Task.Configuration.Prompt);

        await Task.Delay(100, cancellationToken); // Simulate API call

        return "{\"status\": \"success\", \"message\": \"Azure OpenAI task executed (placeholder)\"}";
    }
}
