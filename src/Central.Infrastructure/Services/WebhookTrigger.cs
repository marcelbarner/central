using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using Central.Domain.Webhooks;
using Central.Domain.Webhooks.Ports;

using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services;

/// <summary>
/// Service for triggering webhook HTTP requests.
/// </summary>
public class WebhookTrigger(
    IWebhookRepository webhookRepository,
    IHttpClientFactory httpClientFactory,
    ILogger<WebhookTrigger> logger) : IWebhookTrigger
{
    /// <inheritdoc />
    public async Task TriggerAsync(WebhookEventType eventType, long documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var webhooks = await webhookRepository.GetByEventTypeAsync(eventType, cancellationToken);

            if (!webhooks.Any())
            {
                logger.LogDebug("No webhooks registered for event type {EventType}", eventType);
                return;
            }

            var payload = new WebhookPayload
            {
                EventType = eventType.ToString(),
                DocumentId = documentId,
                Timestamp = DateTimeOffset.UtcNow
            };

            var httpClient = httpClientFactory.CreateClient("WebhookClient");

            // Fire and forget - trigger webhooks in parallel without awaiting
            var tasks = webhooks.Select(webhook => SendWebhookAsync(httpClient, webhook, payload, cancellationToken));

            // Wait for all webhook calls to complete
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Critical error in webhook trigger for event type {EventType}", eventType);
            // Swallow the exception to prevent it from affecting the calling code
        }
    }

    private async Task SendWebhookAsync(HttpClient httpClient, Webhook webhook, WebhookPayload payload, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Triggering webhook {WebhookId} to {Url} for event {EventType}",
                webhook.Id, webhook.Url, payload.EventType);

            var response = await httpClient.PostAsJsonAsync(webhook.Url, payload, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Webhook {WebhookId} triggered successfully", webhook.Id);
            }
            else
            {
                logger.LogWarning("Webhook {WebhookId} failed with status {StatusCode}",
                    webhook.Id, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error triggering webhook {WebhookId} to {Url}", webhook.Id, webhook.Url);
        }
    }
}