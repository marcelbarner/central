using Central.Domain.Webhooks;
using Central.Domain.Webhooks.Services;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Webhooks;

public sealed record ImportWebhooksRequest
{
    public required IFormFile File { get; init; }
}

public sealed record ImportWebhooksResponse
{
    public required int ImportedCount { get; init; }
    public required int SkippedCount { get; init; }
    public required IReadOnlyList<string> Errors { get; init; }
}

public sealed class ImportWebhooksCsvEndpoint(IWebhookService webhookService)
    : Endpoint<ImportWebhooksRequest, ImportWebhooksResponse>
{
    public override void Configure()
    {
        Post("/api/webhooks/import");
        AllowFileUploads();
    }

    public override async Task HandleAsync(ImportWebhooksRequest req, CancellationToken ct)
    {
        var errors = new List<string>();
        var imported = 0;
        var skipped = 0;

        using var reader = new StreamReader(req.File.OpenReadStream());
        
        // Skip header line
        await reader.ReadLineAsync(ct);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(ct);
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var values = ParseCsvLine(line);
            
            // Expected format: EventType,Url,Name,Description
            if (values.Length < 2)
            {
                skipped++;
                errors.Add($"Line skipped: Invalid format (expected: EventType,Url,Name,Description) - {line}");
                continue;
            }

            var eventTypeStr = values[0].Trim();
            var url = values[1].Trim();
            var name = values.Length > 2 ? values[2].Trim() : null;
            var description = values.Length > 3 ? values[3].Trim() : null;

            if (string.IsNullOrWhiteSpace(eventTypeStr) || string.IsNullOrWhiteSpace(url))
            {
                skipped++;
                errors.Add($"Line skipped: EventType and Url are required - {line}");
                continue;
            }

            if (!Enum.TryParse<WebhookEventType>(eventTypeStr, true, out var eventType))
            {
                skipped++;
                errors.Add($"Line skipped: Invalid event type '{eventTypeStr}' - {line}");
                continue;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || 
                (uri.Scheme != "http" && uri.Scheme != "https"))
            {
                skipped++;
                errors.Add($"Line skipped: Invalid URL '{url}' - {line}");
                continue;
            }

            try
            {
                await webhookService.CreateAsync(
                    eventType, 
                    url, 
                    string.IsNullOrWhiteSpace(name) ? null : name,
                    string.IsNullOrWhiteSpace(description) ? null : description,
                    ct);
                imported++;
            }
            catch (Exception ex)
            {
                skipped++;
                errors.Add($"Skipped webhook '{name ?? url}': {ex.Message}");
            }
        }

        await Send.OkAsync(new ImportWebhooksResponse
        {
            ImportedCount = imported,
            SkippedCount = skipped,
            Errors = errors
        }, ct);
    }

    private static string[] ParseCsvLine(string line)
    {
        var values = new List<string>();
        var inQuotes = false;
        var currentValue = string.Empty;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                values.Add(currentValue);
                currentValue = string.Empty;
            }
            else
            {
                currentValue += c;
            }
        }

        values.Add(currentValue);
        return values.ToArray();
    }
}
