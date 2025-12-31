using Central.Domain.Tags.Services;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Tags;

public sealed record ImportTagsRequest
{
    public required IFormFile File { get; init; }
}

public sealed record ImportTagsResponse
{
    public required int ImportedCount { get; init; }
    public required int SkippedCount { get; init; }
    public required IReadOnlyList<string> Errors { get; init; }
}

public sealed class ImportTagsCsvEndpoint(ITagService tagService)
    : Endpoint<ImportTagsRequest, ImportTagsResponse>
{
    public override void Configure()
    {
        Post("/api/tags/import");
        AllowFileUploads();
    }

    public override async Task HandleAsync(ImportTagsRequest req, CancellationToken ct)
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

            if (values.Length < 1)
            {
                skipped++;
                errors.Add($"Line skipped: Invalid format - {line}");
                continue;
            }

            var name = values[0].Trim();
            var description = values.Length > 1 ? values[1].Trim() : null;

            if (string.IsNullOrWhiteSpace(name))
            {
                skipped++;
                errors.Add($"Line skipped: Name is required - {line}");
                continue;
            }

            try
            {
                await tagService.CreateAsync(name, string.IsNullOrWhiteSpace(description) ? null : description, ct);
                imported++;
            }
            catch (InvalidOperationException ex)
            {
                skipped++;
                errors.Add($"Skipped '{name}': {ex.Message}");
            }
        }

        await Send.OkAsync(new ImportTagsResponse
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