using Central.Domain.DocumentTypes.Services;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.DocumentTypes;

public sealed record ImportDocumentTypesRequest
{
    public required IFormFile File { get; init; }
}

public sealed record ImportDocumentTypesResponse
{
    public required int ImportedCount { get; init; }
    public required int SkippedCount { get; init; }
    public required IReadOnlyList<string> Errors { get; init; }
}

public sealed class ImportDocumentTypesCsvEndpoint(IDocumentTypeService documentTypeService)
    : Endpoint<ImportDocumentTypesRequest, ImportDocumentTypesResponse>
{
    public override void Configure()
    {
        Post("/api/document-types/import");
        AllowFileUploads();
    }

    public override async Task HandleAsync(ImportDocumentTypesRequest req, CancellationToken ct)
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
                await documentTypeService.CreateAsync(name, string.IsNullOrWhiteSpace(description) ? null : description, ct);
                imported++;
            }
            catch (InvalidOperationException ex)
            {
                skipped++;
                errors.Add($"Skipped '{name}': {ex.Message}");
            }
        }

        await Send.OkAsync(new ImportDocumentTypesResponse
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