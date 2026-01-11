using System.Text.Json;

using Central.Domain.Ports;

using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for setting the document date.
/// </summary>
public sealed class SetDocumentDateTool : IDocumentTool
{
    private readonly ILogger<SetDocumentDateTool> _logger;

    public SetDocumentDateTool(ILogger<SetDocumentDateTool> logger)
    {
        _logger = logger;
    }

    public string Name => "set_document_date";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var args = JsonSerializer.Deserialize<SetDocumentDateArgs>(arguments, JsonSerializerOptions.Web);
        if (args == null || string.IsNullOrWhiteSpace(args.DocumentDate))
        {
            return "Error: Document date cannot be empty";
        }

        if (!DateTimeOffset.TryParse(args.DocumentDate, out var parsedDate))
        {
            return $"Error: Invalid date format. Expected ISO 8601 format, got: {args.DocumentDate}";
        }

        var updatedDocument = context.Document with { DocumentDate = parsedDate };
        await context.DocumentRepository.UpdateAsync(updatedDocument, cancellationToken);

        _logger.LogInformation("Document {DocumentId} date updated to: {DocumentDate}", context.Document.Id, parsedDate);
        return $"Document date successfully updated to: {parsedDate:yyyy-MM-dd}";
    }

    private sealed class SetDocumentDateArgs
    {
        public string DocumentDate { get; set; } = string.Empty;
    }
}
